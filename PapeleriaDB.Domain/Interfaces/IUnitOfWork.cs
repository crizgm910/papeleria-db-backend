namespace PapeleriaDB.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IProductoRepository Productos { get; }
    IVentaRepository Ventas { get; }
    ICajaRepository Cajas { get; }
    IUsuarioRepository Usuarios { get; }
    IServicioRepository Servicios { get; }
    ICategoriaRepository Categorias { get; }
    IMovimientoInventarioRepository MovimientosInventario { get; }
    IAuditoriaRepository Auditorias { get; }
    IProveedorRepository Proveedores { get; }
    ICompraRepository Compras { get; }
    
    Task<int> CompleteAsync();
    Task ExecuteInTransactionAsync(Func<Task> action);
}
