using PapeleriaDB.Domain.Entities;

namespace PapeleriaDB.Domain.Interfaces;

public interface ICajaRepository : IRepository<Caja>
{
    Task<Caja?> GetCajaConActividadDesdeAsync(int cajaId, DateTime fechaDesde);
    Task<CorteCaja?> GetUltimoCorteAbiertoAsync(int cajaId);
    Task<IEnumerable<Caja>> GetAllWithActivityAsync();
}
