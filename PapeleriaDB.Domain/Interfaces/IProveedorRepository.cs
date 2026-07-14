using PapeleriaDB.Domain.Entities;

namespace PapeleriaDB.Domain.Interfaces;

public interface IProveedorRepository : IRepository<Proveedor>
{
    Task<Proveedor?> GetByRfcAsync(string rfc);
}
