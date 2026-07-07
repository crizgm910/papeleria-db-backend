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

    public async Task<Caja?> GetCajaConVentasDelDiaAsync(int cajaId, DateTime fecha)
    {
        return await _context.Cajas
            .Include(c => c.Ventas.Where(v => v.Fecha.Date == fecha.Date && v.Estado == "Completada"))
            .FirstOrDefaultAsync(c => c.Id == cajaId);
    }

    public async Task<CorteCaja?> GetUltimoCorteAbiertoAsync(int cajaId)
    {
        return await _context.CortesCaja
            .Where(c => c.CajaId == cajaId && c.Estado == "Abierto")
            .OrderByDescending(c => c.FechaApertura)
            .FirstOrDefaultAsync();
    }
}
