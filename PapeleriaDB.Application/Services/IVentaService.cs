using PapeleriaDB.Application.DTOs;

namespace PapeleriaDB.Application.Services;

public interface IVentaService
{
    Task<VentaResponseDto> RegistrarVentaAsync(CrearVentaDto dto);
    Task<DevolucionResponseDto> DevolverArticulosAsync(
        int ventaId,
        List<DevolucionItemDto> devoluciones,
        int? cajaReembolsoId = null,
        int? usuarioId = null);
    Task<IEnumerable<VentaHistorialDto>> GetRecentAsync(int limit);
}
