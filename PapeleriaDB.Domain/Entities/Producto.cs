using System.ComponentModel.DataAnnotations;

namespace PapeleriaDB.Domain.Entities;

public class Producto
{
    public int Id { get; set; }
    public string CodigoInterno { get; set; } = string.Empty;
    public string? CodigoBarras { get; set; }
    public string? CodigoQR { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }
    public decimal PrecioVenta { get; set; }
    public decimal CostoCompra { get; set; }
    [ConcurrencyCheck]
    public int StockActual { get; set; }
    public int StockMinimo { get; set; }
    public bool Estado { get; set; } = true;
    public string? Descripcion { get; set; }
}
