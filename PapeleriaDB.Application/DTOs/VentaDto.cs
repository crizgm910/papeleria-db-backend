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
    public decimal Descuento { get; set; } = 0;
    public List<CrearDetalleVentaDto> Detalles { get; set; } = new();
}

public class CrearDetalleVentaDto
{
    public int? ProductoId { get; set; }
    public int? ServicioId { get; set; }
    public int Cantidad { get; set; }
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

public class DevolucionItemDto
{
    public int DetalleVentaId { get; set; }
    public int CantidadDevolver { get; set; }
}

public class DevolucionResponseDto
{
    public bool Exito { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public decimal MontoReembolsado { get; set; }
}
