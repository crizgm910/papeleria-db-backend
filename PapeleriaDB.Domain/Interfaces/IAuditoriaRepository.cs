using PapeleriaDB.Domain.Entities;

namespace PapeleriaDB.Domain.Interfaces;

public interface IAuditoriaRepository : IRepository<Auditoria>
{
    Task<IEnumerable<Auditoria>> GetRecentAsync(int limit, string? recurso);
}
