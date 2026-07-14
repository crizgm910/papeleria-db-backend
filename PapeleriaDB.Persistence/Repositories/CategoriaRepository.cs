using Microsoft.EntityFrameworkCore;
using PapeleriaDB.Domain.Entities;
using PapeleriaDB.Domain.Interfaces;
using PapeleriaDB.Persistence.Contexts;

namespace PapeleriaDB.Persistence.Repositories;

public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
{
    public CategoriaRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Categoria?> GetByNombreAsync(string nombre)
    {
        var nombreNormalizado = nombre.Trim().ToLower();
        return await _context.Categorias
            .FirstOrDefaultAsync(c => c.Nombre.ToLower() == nombreNormalizado);
    }
}
