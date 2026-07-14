namespace PapeleriaDB.Application.DTOs;

public class MobileDashboardDto
{
    public DateTime Fecha { get; set; }
    public ResumenVentasHoyDto VentasHoy { get; set; } = new();
    public int TotalAlertasStock { get; set; }
    public int TotalServiciosActivos { get; set; }
    public int TotalMovimientosHoy { get; set; }
    public IEnumerable<ProductoDto> ProductosBajoStock { get; set; } = [];
    public IEnumerable<MovimientoInventarioDto> MovimientosRecientes { get; set; } = [];
}

public class ResumenVentasHoyDto
{
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public int TotalVentas { get; set; }
    public decimal IngresoTotal { get; set; }
}
