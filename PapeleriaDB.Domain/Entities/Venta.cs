namespace PapeleriaDB.Domain.Entities;

public class Venta
{
    public int Id { get; set; }
    public string Folio { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public int CajaId { get; set; }
    public Caja? Caja { get; set; }
    public int UsuarioId { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Descuento { get; set; }
    public decimal Total { get; set; }
    public string Estado { get; set; } = "Completada"; // Completada, Cancelada
    public string MetodoPagoPrincipal { get; set; } = "Efectivo";

    public ICollection<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();
}
