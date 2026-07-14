using PapeleriaDB.Application.DTOs;

namespace PapeleriaDB.Application.Services;

public interface ICompraService
{
    Task<IEnumerable<CompraDto>> GetRecentAsync(int limit);
    Task<CompraDto> GetByIdAsync(int id);
    Task<CompraDto> RegisterAsync(RegistrarCompraDto dto, int usuarioId);
}
