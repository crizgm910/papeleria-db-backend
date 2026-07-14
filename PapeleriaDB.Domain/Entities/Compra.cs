namespace PapeleriaDB.Domain.Entities;

public class Compra
{
    public int Id { get; set; }
    public int ProveedorId { get; set; }
    public Proveedor? Proveedor { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
    public decimal Total { get; set; }
    public string FolioProveedor { get; set; } = string.Empty;
    public string? Notas { get; set; }
    public int UsuarioId { get; set; }
    public ICollection<DetalleCompra> Detalles { get; set; } = new List<DetalleCompra>();
}
