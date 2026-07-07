namespace PapeleriaDB.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IProductoRepository Productos { get; }
    IVentaRepository Ventas { get; }
    ICajaRepository Cajas { get; }
    IUsuarioRepository Usuarios { get; }
    
    Task<int> CompleteAsync();
}
