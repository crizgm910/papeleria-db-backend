using PapeleriaDB.Application.DTOs;

namespace PapeleriaDB.Application.Services;

public interface IMovimientoCajaService
{
    Task<MovimientoCajaDto> RegistrarAsync(RegistrarMovimientoCajaDto dto, int usuarioId);
    Task<IEnumerable<MovimientoCajaDto>> GetRecentAsync(int cajaId, int limit);
}
