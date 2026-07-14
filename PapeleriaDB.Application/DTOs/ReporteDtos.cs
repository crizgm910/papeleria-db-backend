namespace PapeleriaDB.Application.DTOs;

public class ProductoTopDto
{
    public int ProductoId { get; set; }
    public string ProductoNombre { get; set; } = string.Empty;
    public int CantidadTotal { get; set; }
    public decimal IngresoTotal { get; set; }
}

public class ResumenVentasDto
{
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public int TotalVentas { get; set; }
    public decimal IngresoTotal { get; set; }
    public decimal TicketPromedio { get; set; }
}
