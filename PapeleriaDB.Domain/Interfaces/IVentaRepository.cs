using PapeleriaDB.Domain.Entities;

namespace PapeleriaDB.Domain.Interfaces;

public interface IVentaRepository : IRepository<Venta>
{
    Task<Venta?> GetVentaConDetallesAsync(int id);
    Task<IEnumerable<Venta>> GetVentasPorCajaAsync(int cajaId, DateTime fecha);
    Task<IEnumerable<Venta>> GetVentasConDetallesAsync(DateTime fechaInicio, DateTime fechaFin);
    Task<IEnumerable<Venta>> GetRecentWithDetailsAsync(int limit);
}
