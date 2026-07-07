using PapeleriaDB.Application.DTOs;

namespace PapeleriaDB.Application.Services;

public interface IProductoService
{
    Task<IEnumerable<ProductoDto>> GetAllAsync();
    Task<ProductoDto?> GetByIdAsync(int id);
    Task<ProductoDto?> GetByCodigoBarrasAsync(string codigoBarras);
    Task<ProductoDto> CreateAsync(CrearProductoDto dto);
}
