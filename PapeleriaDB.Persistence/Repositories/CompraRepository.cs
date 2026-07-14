using Microsoft.EntityFrameworkCore;
using PapeleriaDB.Domain.Entities;
using PapeleriaDB.Domain.Interfaces;
using PapeleriaDB.Persistence.Contexts;

namespace PapeleriaDB.Persistence.Repositories;

public class CompraRepository : Repository<Compra>, ICompraRepository
{
    public CompraRepository(ApplicationDbContext context) : base(context) { }
    public async Task<IEnumerable<Compra>> GetRecentAsync(int limit) => await _context.Compras.AsNoTracking()
        .Include(c => c.Proveedor).Include(c => c.Detalles).ThenInclude(d => d.Producto)
        .OrderByDescending(c => c.Fecha).Take(limit).ToListAsync();
    public Task<Compra?> GetDetailAsync(int id) => _context.Compras.AsNoTracking()
        .Include(c => c.Proveedor).Include(c => c.Detalles).ThenInclude(d => d.Producto)
        .FirstOrDefaultAsync(c => c.Id == id);
}
