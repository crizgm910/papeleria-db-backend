using PapeleriaDB.Domain.Entities;

namespace PapeleriaDB.Domain.Interfaces;

public interface ICajaRepository : IRepository<Caja>
{
    Task<Caja?> GetCajaConVentasDelDiaAsync(int cajaId, DateTime fecha);
    Task<CorteCaja?> GetUltimoCorteAbiertoAsync(int cajaId);
    Task<IEnumerable<Caja>> GetAllWithActivityAsync();
}
