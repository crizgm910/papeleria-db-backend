using PapeleriaDB.Application.DTOs;
using PapeleriaDB.Domain.Entities;
using PapeleriaDB.Domain.Interfaces;

namespace PapeleriaDB.Application.Services;

public class AuditoriaService : IAuditoriaService
{
    private readonly IUnitOfWork _unitOfWork;
    public AuditoriaService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task RecordAsync(int usuarioId, string usuarioNombre, string accion, string recurso, string? recursoId, string metodo, string ruta, int estadoHttp)
    {
        var usuario = await _unitOfWork.Usuarios.GetByIdAsync(usuarioId);
        var nombreVisible = usuario?.NombreCompleto ?? usuarioNombre;
        await _unitOfWork.Auditorias.AddAsync(new Auditoria { UsuarioId = usuarioId, UsuarioNombre = nombreVisible, Accion = accion, Recurso = recurso, RecursoId = recursoId, Metodo = metodo, Ruta = ruta, EstadoHttp = estadoHttp, Fecha = DateTime.UtcNow });
        await _unitOfWork.CompleteAsync();
    }

    public async Task<IEnumerable<AuditoriaDto>> GetRecentAsync(int limit, string? recurso) =>
        (await _unitOfWork.Auditorias.GetRecentAsync(limit, recurso)).Select(a => new AuditoriaDto { Id = a.Id, UsuarioId = a.UsuarioId, UsuarioNombre = a.UsuarioNombre, Accion = a.Accion, Recurso = a.Recurso, RecursoId = a.RecursoId, Metodo = a.Metodo, Ruta = a.Ruta, EstadoHttp = a.EstadoHttp, Fecha = a.Fecha.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(a.Fecha, DateTimeKind.Utc) : a.Fecha });
}
