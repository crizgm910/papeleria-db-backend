
using PapeleriaDB.Application.DTOs;
using PapeleriaDB.Domain.Interfaces;

namespace PapeleriaDB.Application.Services;

public class ReportesService : IReportesService
{
    private readonly IUnitOfWork _unitOfWork;

    public ReportesService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<ProductoDto>> GetProductosConBajoStockAsync(int limiteStock = 5)
    {
        var productos = await _unitOfWork.Productos.GetAllAsync();
        var productosBajoStock = productos.Where(p => p.StockActual <= limiteStock).ToList();
        
        return productosBajoStock.Select(p => new ProductoDto
        {
            Id = p.Id,
            CodigoBarras = p.CodigoBarras ?? p.CodigoInterno,
            Nombre = p.Nombre,
            PrecioVenta = p.PrecioVenta,
            StockActual = p.StockActual
        });
    }

    public async Task<IEnumerable<ProductoTopDto>> GetTopProductosMasVendidosAsync(int top = 5)
    {
        // En un escenario real con muchos datos esto debería ir a un Repositorio específico 
        // usando IQueryable para no traer todo a memoria, pero como MVP usamos el GetAll.
        var ventas = await _unitOfWork.Ventas.GetVentasConDetallesAsync(DateTime.MinValue, DateTime.MaxValue.AddDays(-1));
        
        var topVendidos = ventas
            .SelectMany(v => v.Detalles.Where(d => d.ProductoId.HasValue))
            .GroupBy(d => new { ProductoId = d.ProductoId!.Value, Nombre = d.Producto?.Nombre ?? d.Descripcion })
            .Select(g => new ProductoTopDto
            {
                ProductoId = g.Key.ProductoId,
                ProductoNombre = g.Key.Nombre,
                CantidadTotal = g.Sum(d => d.Cantidad),
                IngresoTotal = g.Sum(d => d.Subtotal)
            })
            .OrderByDescending(x => x.CantidadTotal)
            .Take(top)
            .ToList();

        return topVendidos;
    }

    public async Task<ResumenVentasDto> GetResumenVentasPorFechaAsync(DateTime fechaInicio, DateTime fechaFin)
    {
        var ventasFiltradas = (await _unitOfWork.Ventas.GetVentasConDetallesAsync(fechaInicio, fechaFin)).ToList();
        var ingreso = ventasFiltradas.Sum(v => v.Total);

        return new ResumenVentasDto
        {
            FechaInicio = fechaInicio.Date,
            FechaFin = fechaFin.Date,
            TotalVentas = ventasFiltradas.Count,
            IngresoTotal = ingreso,
            TicketPromedio = ventasFiltradas.Count == 0 ? 0 : ingreso / ventasFiltradas.Count
        };
    }
}
