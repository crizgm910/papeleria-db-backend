namespace PapeleriaDB.Domain.Entities;

public class DetalleVenta
{
    public int Id { get; set; }
    public int VentaId { get; set; }
    public Venta? Venta { get; set; }
    
    // Puede ser "Producto" o "Servicio"
    public string TipoItem { get; set; } = "Producto";
    
    public int? ProductoId { get; set; }
    public Producto? Producto { get; set; }
    
    public int? ServicioId { get; set; }
    
    public string Descripcion { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }
}
