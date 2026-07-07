
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

    public async Task<IEnumerable<object>> GetTopProductosMasVendidosAsync(int top = 5)
    {
        // En un escenario real con muchos datos esto debería ir a un Repositorio específico 
        // usando IQueryable para no traer todo a memoria, pero como MVP usamos el GetAll.
        var ventas = await _unitOfWork.Ventas.GetAllAsync();
        
        var topVendidos = ventas
            .SelectMany(v => v.Detalles)
            .GroupBy(d => new { d.ProductoId, d.Producto.Nombre })
            .Select(g => new
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

    public async Task<object> GetResumenVentasPorFechaAsync(DateTime fechaInicio, DateTime fechaFin)
    {
        var ventas = await _unitOfWork.Ventas.GetAllAsync();
        
        var ventasFiltradas = ventas
            .Where(v => v.Fecha.Date >= fechaInicio.Date && v.Fecha.Date <= fechaFin.Date)
            .ToList();

        return new
        {
            FechaInicio = fechaInicio.Date,
            FechaFin = fechaFin.Date,
            TotalVentas = ventasFiltradas.Count,
            IngresoTotal = ventasFiltradas.Sum(v => v.Total)
        };
    }
}
