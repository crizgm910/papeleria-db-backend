using PapeleriaDB.Application.DTOs;

namespace PapeleriaDB.Application.Services;

public interface IServicioService
{
    Task<IEnumerable<ServicioDto>> GetAllAsync();
    Task<ServicioDto> CreateAsync(GuardarServicioDto dto);
    Task<ServicioDto> UpdateAsync(int id, GuardarServicioDto dto);
    Task DeleteAsync(int id);
}
