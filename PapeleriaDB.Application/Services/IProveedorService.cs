using PapeleriaDB.Application.DTOs;

namespace PapeleriaDB.Application.Services;

public interface IProveedorService
{
    Task<IEnumerable<ProveedorDto>> GetAllAsync();
    Task<ProveedorDto> CreateAsync(GuardarProveedorDto dto);
    Task<ProveedorDto> UpdateAsync(int id, GuardarProveedorDto dto);
    Task DeactivateAsync(int id);
}
