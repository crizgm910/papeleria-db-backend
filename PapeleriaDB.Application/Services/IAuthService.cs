using PapeleriaDB.Application.DTOs;

namespace PapeleriaDB.Application.Services;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
}
