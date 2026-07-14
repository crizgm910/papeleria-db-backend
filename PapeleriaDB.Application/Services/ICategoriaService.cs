using PapeleriaDB.Application.DTOs;

namespace PapeleriaDB.Application.Services;

public interface ICategoriaService
{
    Task<IEnumerable<CategoriaDto>> GetAllAsync();
    Task<CategoriaDto> CreateAsync(CrearCategoriaDto dto);
}
