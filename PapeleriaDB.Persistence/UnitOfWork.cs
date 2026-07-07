using PapeleriaDB.Domain.Interfaces;
using PapeleriaDB.Persistence.Contexts;
using PapeleriaDB.Persistence.Repositories;

namespace PapeleriaDB.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    
    public IProductoRepository Productos { get; private set; }
    public IVentaRepository Ventas { get; private set; }
    public ICajaRepository Cajas { get; private set; }
    public IUsuarioRepository Usuarios { get; private set; }

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Productos = new ProductoRepository(_context);
        Ventas = new VentaRepository(_context);
        Cajas = new CajaRepository(_context);
        Usuarios = new UsuarioRepository(_context);
    }

    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
