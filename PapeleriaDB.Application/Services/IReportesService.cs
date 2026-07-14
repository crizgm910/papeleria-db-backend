using PapeleriaDB.Application.DTOs;

namespace PapeleriaDB.Application.Services;

public interface IReportesService
{
    Task<IEnumerable<ProductoDto>> GetProductosConBajoStockAsync(int limiteStock = 5);
    Task<IEnumerable<ProductoTopDto>> GetTopProductosMasVendidosAsync(int top = 5);
    Task<ResumenVentasDto> GetResumenVentasPorFechaAsync(DateTime fechaInicio, DateTime fechaFin);
}
