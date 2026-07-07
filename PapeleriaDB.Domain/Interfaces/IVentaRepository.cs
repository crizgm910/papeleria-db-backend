using PapeleriaDB.Domain.Entities;

namespace PapeleriaDB.Domain.Interfaces;

public interface IVentaRepository : IRepository<Venta>
{
    Task<Venta?> GetVentaConDetallesAsync(int id);
    Task<IEnumerable<Venta>> GetVentasPorCajaAsync(int cajaId, DateTime fecha);
}
