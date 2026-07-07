using PapeleriaDB.Application.DTOs;

namespace PapeleriaDB.Application.Services;

public interface IReportesService
{
    Task<IEnumerable<ProductoDto>> GetProductosConBajoStockAsync(int limiteStock = 5);
    Task<IEnumerable<object>> GetTopProductosMasVendidosAsync(int top = 5);
    Task<object> GetResumenVentasPorFechaAsync(DateTime fechaInicio, DateTime fechaFin);
}
