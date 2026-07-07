using PapeleriaDB.Application.DTOs;

namespace PapeleriaDB.Application.Services;

public interface IVentaService
{
    Task<VentaResponseDto> RegistrarVentaAsync(CrearVentaDto dto);
}
