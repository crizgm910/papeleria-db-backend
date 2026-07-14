using PapeleriaDB.Domain.Entities;

namespace PapeleriaDB.Domain.Interfaces;

public interface IMovimientoInventarioRepository : IRepository<MovimientoInventario>
{
    Task<IEnumerable<MovimientoInventario>> GetRecentAsync(int limit);
    Task<int> CountBetweenAsync(DateTime start, DateTime end);
}
