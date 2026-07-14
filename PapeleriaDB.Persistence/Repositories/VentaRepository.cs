using Microsoft.EntityFrameworkCore;
using PapeleriaDB.Domain.Entities;
using PapeleriaDB.Domain.Interfaces;
using PapeleriaDB.Persistence.Contexts;

namespace PapeleriaDB.Persistence.Repositories;

public class VentaRepository : Repository<Venta>, IVentaRepository
{
    public VentaRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Venta?> GetVentaConDetallesAsync(int id)
    {
        return await _context.Ventas
            .Include(v => v.Detalles)
            .ThenInclude(d => d.Producto)
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<IEnumerable<Venta>> GetVentasPorCajaAsync(int cajaId, DateTime fecha)
    {
        return await _context.Ventas
            .Where(v => v.CajaId == cajaId && v.Fecha.Date == fecha.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<Venta>> GetVentasConDetallesAsync(DateTime fechaInicio, DateTime fechaFin)
    {
        var finExclusivo = fechaFin.Date.AddDays(1);
        return await _context.Ventas
            .AsNoTracking()
            .Include(v => v.Detalles)
            .ThenInclude(d => d.Producto)
            .Where(v => v.Fecha >= fechaInicio.Date && v.Fecha < finExclusivo && v.Estado == "Completada")
            .ToListAsync();
    }

    public async Task<IEnumerable<Venta>> GetRecentWithDetailsAsync(int limit)
    {
        return await _context.Ventas.AsNoTracking().Include(v => v.Detalles)
            .OrderByDescending(v => v.Fecha).Take(limit).ToListAsync();
    }
}
