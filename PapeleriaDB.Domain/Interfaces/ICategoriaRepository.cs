using PapeleriaDB.Domain.Entities;

namespace PapeleriaDB.Domain.Interfaces;

public interface ICategoriaRepository : IRepository<Categoria>
{
    Task<Categoria?> GetByNombreAsync(string nombre);
}
