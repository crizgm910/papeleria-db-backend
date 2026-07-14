namespace PapeleriaDB.Application.DTOs;

public class AuditoriaDto
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public string UsuarioNombre { get; set; } = string.Empty;
    public string Accion { get; set; } = string.Empty;
    public string Recurso { get; set; } = string.Empty;
    public string? RecursoId { get; set; }
    public string Metodo { get; set; } = string.Empty;
    public string Ruta { get; set; } = string.Empty;
    public int EstadoHttp { get; set; }
    public DateTime Fecha { get; set; }
}
