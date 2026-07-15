using PapeleriaDB.Application.DTOs;

namespace PapeleriaDB.Application.Services;

public interface ICajaService
{
    Task<IEnumerable<CajaDto>> GetAllAsync();
    Task<CajaDto> CreateAsync(CrearCajaDto dto);
    Task<CorteCajaResponseDto> AbrirCajaAsync(AbrirCajaDto dto);
    Task<CorteCajaResponseDto> CerrarCajaAsync(CerrarCajaDto dto);
    Task<IEnumerable<CajaSupervisionDto>> GetSupervisionAsync(int historyLimit);
}
