using Microsoft.EntityFrameworkCore;
using PapeleriaDB.Domain.Entities;
using PapeleriaDB.Domain.Interfaces;
using PapeleriaDB.Persistence.Contexts;

namespace PapeleriaDB.Persistence.Repositories;

public class ProveedorRepository : Repository<Proveedor>, IProveedorRepository
{
    public ProveedorRepository(ApplicationDbContext context) : base(context) { }
    public Task<Proveedor?> GetByRfcAsync(string rfc) =>
        _context.Proveedores.FirstOrDefaultAsync(p => p.Rfc == rfc);
}
