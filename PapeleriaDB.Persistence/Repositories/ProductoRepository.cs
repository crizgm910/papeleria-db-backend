using Microsoft.EntityFrameworkCore;
using PapeleriaDB.Domain.Entities;
using PapeleriaDB.Domain.Interfaces;
using PapeleriaDB.Persistence.Contexts;

namespace PapeleriaDB.Persistence.Repositories;

public class ProductoRepository : Repository<Producto>, IProductoRepository
{
    public ProductoRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Producto?> GetByCodigoInternoAsync(string codigoInterno)
    {
        return await _context.Productos.FirstOrDefaultAsync(p => p.CodigoInterno == codigoInterno);
    }

    public async Task<Producto?> GetByCodigoBarrasAsync(string codigoBarras)
    {
        return await _context.Productos.FirstOrDefaultAsync(p => p.CodigoBarras == codigoBarras);
    }

    public async Task<IEnumerable<Producto>> GetByCategoriaAsync(int categoriaId)
    {
        return await _context.Productos.Where(p => p.CategoriaId == categoriaId).ToListAsync();
    }

    public async Task<IEnumerable<Producto>> GetLowStockAsync()
    {
        return await _context.Productos.Where(p => p.StockActual <= p.StockMinimo).ToListAsync();
    }
}
