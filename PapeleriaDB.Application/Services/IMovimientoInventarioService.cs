using PapeleriaDB.Application.DTOs;

namespace PapeleriaDB.Application.Services;

public interface IMovimientoInventarioService
{
    Task<IEnumerable<MovimientoInventarioDto>> GetRecentAsync(int limit);
    Task<MovimientoInventarioDto> RegisterAsync(RegistrarMovimientoInventarioDto dto, int usuarioId);
}
