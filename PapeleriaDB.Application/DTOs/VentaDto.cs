namespace PapeleriaDB.Application.DTOs;

public class DetalleVentaDto
{
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
}

public class CrearVentaDto
{
    public int CajaId { get; set; }
    public int UsuarioId { get; set; }
    public string MetodoPagoPrincipal { get; set; } = "Efectivo";
    public List<DetalleVentaDto> Detalles { get; set; } = new();
}

public class VentaResponseDto
{
    public int Id { get; set; }
    public string Folio { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public string Estado { get; set; } = string.Empty;
    public bool Exito { get; set; }
    public string Mensaje { get; set; } = string.Empty;
}
