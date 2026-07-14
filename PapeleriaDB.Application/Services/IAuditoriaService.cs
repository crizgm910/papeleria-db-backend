using PapeleriaDB.Application.DTOs;

namespace PapeleriaDB.Application.Services;

public interface IAuditoriaService
{
    Task RecordAsync(int usuarioId, string usuarioNombre, string accion, string recurso, string? recursoId, string metodo, string ruta, int estadoHttp);
    Task<IEnumerable<AuditoriaDto>> GetRecentAsync(int limit, string? recurso);
}
