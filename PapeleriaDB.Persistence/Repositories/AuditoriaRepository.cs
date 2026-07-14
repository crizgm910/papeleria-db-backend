using Microsoft.EntityFrameworkCore;
using PapeleriaDB.Domain.Entities;
using PapeleriaDB.Domain.Interfaces;
using PapeleriaDB.Persistence.Contexts;

namespace PapeleriaDB.Persistence.Repositories;

public class AuditoriaRepository : Repository<Auditoria>, IAuditoriaRepository
{
    public AuditoriaRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Auditoria>> GetRecentAsync(int limit, string? recurso)
    {
        var query = _context.Auditorias.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(recurso)) query = query.Where(a => a.Recurso == recurso);
        return await query.OrderByDescending(a => a.Fecha).Take(Math.Clamp(limit, 1, 500)).ToListAsync();
    }
}
