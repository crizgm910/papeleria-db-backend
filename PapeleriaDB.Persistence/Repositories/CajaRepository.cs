using Microsoft.EntityFrameworkCore;
using PapeleriaDB.Domain.Entities;
using PapeleriaDB.Domain.Interfaces;
using PapeleriaDB.Persistence.Contexts;

namespace PapeleriaDB.Persistence.Repositories;

public class CajaRepository : Repository<Caja>, ICajaRepository
{
    public CajaRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Caja?> GetCajaConActividadDesdeAsync(int cajaId, DateTime fechaDesde)
    {
        return await _context.Cajas
            .Include(c => c.Ventas.Where(v => v.Fecha >= fechaDesde && v.Estado == "Completada"))
            .Include(c => c.Movimientos.Where(m => m.Fecha >= fechaDesde))
            .FirstOrDefaultAsync(c => c.Id == cajaId);
    }

    public async Task<CorteCaja?> GetUltimoCorteAbiertoAsync(int cajaId)
    {
        return await _context.CortesCaja
            .Where(c => c.CajaId == cajaId && c.Estado == "Abierto")
            .OrderByDescending(c => c.FechaApertura)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Caja>> GetAllWithActivityAsync()
    {
        return await _context.Cajas.AsNoTracking()
            .Include(c => c.Cortes)
            .Include(c => c.Ventas).ThenInclude(v => v.Detalles)
            .Include(c => c.Movimientos)
            .OrderBy(c => c.Nombre)
            .ToListAsync();
    }
}
