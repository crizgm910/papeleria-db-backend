using PapeleriaDB.Domain.Entities;

namespace PapeleriaDB.Domain.Interfaces;

public interface ICompraRepository : IRepository<Compra>
{
    Task<IEnumerable<Compra>> GetRecentAsync(int limit);
    Task<Compra?> GetDetailAsync(int id);
}
