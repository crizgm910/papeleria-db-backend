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
    public IServicioRepository Servicios { get; private set; }
    public ICategoriaRepository Categorias { get; private set; }
    public IMovimientoInventarioRepository MovimientosInventario { get; private set; }
    public IAuditoriaRepository Auditorias { get; private set; }
    public IProveedorRepository Proveedores { get; private set; }
    public ICompraRepository Compras { get; private set; }

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Productos = new ProductoRepository(_context);
        Ventas = new VentaRepository(_context);
        Cajas = new CajaRepository(_context);
        Usuarios = new UsuarioRepository(_context);
        Servicios = new ServicioRepository(_context);
        Categorias = new CategoriaRepository(_context);
        MovimientosInventario = new MovimientoInventarioRepository(_context);
        Auditorias = new AuditoriaRepository(_context);
        Proveedores = new ProveedorRepository(_context);
        Compras = new CompraRepository(_context);
    }

    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task ExecuteInTransactionAsync(Func<Task> action)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            await action();
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
