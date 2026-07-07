using PapeleriaDB.Domain.Entities;

namespace PapeleriaDB.Domain.Interfaces;

public interface IProductoRepository : IRepository<Producto>
{
    Task<Producto?> GetByCodigoBarrasAsync(string codigoBarras);
    Task<IEnumerable<Producto>> GetByCategoriaAsync(int categoriaId);
    Task<IEnumerable<Producto>> GetLowStockAsync();
}
