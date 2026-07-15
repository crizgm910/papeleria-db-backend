using PapeleriaDB.Application.DTOs;
using PapeleriaDB.Domain.Interfaces;

namespace PapeleriaDB.Application.Services;

public class MobileDashboardService : IMobileDashboardService
{
    private readonly IUnitOfWork _unitOfWork;

    public MobileDashboardService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<MobileDashboardDto> GetAsync()
    {
        var zona = GetMexicoTimeZone();
        var nowUtc = DateTime.UtcNow;
        var now = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, zona);
        var start = DateTime.SpecifyKind(now.Date, DateTimeKind.Unspecified);
        var end = start.AddDays(1);
        var startUtc = TimeZoneInfo.ConvertTimeToUtc(start, zona);
        var endUtc = TimeZoneInfo.ConvertTimeToUtc(end, zona);

        // El DbContext de la unidad de trabajo es compartido: las consultas se ejecutan
        // en secuencia para respetar la seguridad de hilos de Entity Framework.
        var ventas = await _unitOfWork.Ventas.GetAllAsync();
        var productos = await _unitOfWork.Productos.GetAllAsync();
        var servicios = await _unitOfWork.Servicios.GetAllAsync();
        var movimientos = await _unitOfWork.MovimientosInventario.GetRecentAsync(5);
        var totalMovimientos = await _unitOfWork.MovimientosInventario.CountBetweenAsync(startUtc, endUtc);

        var ventasHoy = ventas
            .Where(v => ToUtc(v.Fecha) >= startUtc && ToUtc(v.Fecha) < endUtc && v.Estado != "Cancelada")
            .ToList();
        var bajoStock = productos
            .Where(p => p.Estado && p.StockActual <= p.StockMinimo)
            .OrderBy(p => p.StockActual)
            .ThenBy(p => p.Nombre)
            .ToList();

        return new MobileDashboardDto
        {
            Fecha = now,
            VentasHoy = new ResumenVentasHoyDto
            {
                FechaInicio = start,
                FechaFin = now,
                TotalVentas = ventasHoy.Count,
                IngresoTotal = ventasHoy.Sum(v => v.Total)
            },
            TotalAlertasStock = bajoStock.Count,
            TotalServiciosActivos = servicios.Count(s => s.Estado),
            TotalMovimientosHoy = totalMovimientos,
            ProductosBajoStock = bajoStock.Take(3).Select(p => new ProductoDto
            {
                Id = p.Id,
                CodigoInterno = p.CodigoInterno,
                CodigoBarras = p.CodigoBarras ?? p.CodigoInterno,
                Nombre = p.Nombre,
                CategoriaId = p.CategoriaId,
                PrecioVenta = p.PrecioVenta,
                CostoCompra = p.CostoCompra,
                StockActual = p.StockActual,
                StockMinimo = p.StockMinimo,
                Descripcion = p.Descripcion
            }),
            MovimientosRecientes = movimientos.Select(m => new MovimientoInventarioDto
            {
                Id = m.Id,
                ProductoId = m.ProductoId,
                ProductoNombre = m.Producto?.Nombre ?? "Producto",
                Tipo = m.Tipo,
                Cantidad = m.Cantidad,
                StockAnterior = m.StockAnterior,
                StockNuevo = m.StockNuevo,
                Motivo = m.Motivo,
                Fecha = m.Fecha.Kind == DateTimeKind.Unspecified
                    ? DateTime.SpecifyKind(m.Fecha, DateTimeKind.Utc)
                    : m.Fecha.ToUniversalTime()
            })
        };
    }

    private static TimeZoneInfo GetMexicoTimeZone()
    {
        try { return TimeZoneInfo.FindSystemTimeZoneById("America/Mexico_City"); }
        catch (TimeZoneNotFoundException) { return TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time (Mexico)"); }
    }

    private static DateTime ToUtc(DateTime value) => value.Kind switch
    {
        DateTimeKind.Utc => value,
        DateTimeKind.Local => value.ToUniversalTime(),
        _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
    };
}
