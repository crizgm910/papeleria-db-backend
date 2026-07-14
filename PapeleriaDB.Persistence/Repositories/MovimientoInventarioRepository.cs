using Microsoft.EntityFrameworkCore;
using PapeleriaDB.Domain.Entities;
using PapeleriaDB.Domain.Interfaces;
using PapeleriaDB.Persistence.Contexts;

namespace PapeleriaDB.Persistence.Repositories;

public class MovimientoInventarioRepository : Repository<MovimientoInventario>, IMovimientoInventarioRepository
{
    public MovimientoInventarioRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<MovimientoInventario>> GetRecentAsync(int limit) => await _context.MovimientosInventario
        .AsNoTracking().Include(m => m.Producto).OrderByDescending(m => m.Fecha).Take(limit).ToListAsync();

    public Task<int> CountBetweenAsync(DateTime start, DateTime end) =>
        _context.MovimientosInventario.CountAsync(m => m.Fecha >= start && m.Fecha < end);
}
