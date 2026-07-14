namespace PapeleriaDB.Application.DTOs;

public class ProductoDto
{
    public int Id { get; set; }
    public string CodigoInterno { get; set; } = string.Empty;
    public string CodigoBarras { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int CategoriaId { get; set; }
    public decimal PrecioVenta { get; set; }
    public decimal CostoCompra { get; set; }
    public int StockActual { get; set; }
    public int StockMinimo { get; set; }
    public string? Descripcion { get; set; }
}

public class CrearProductoDto
{
    public string CodigoInterno { get; set; } = string.Empty;
    public string? CodigoBarras { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int CategoriaId { get; set; }
    public decimal PrecioVenta { get; set; }
    public decimal CostoCompra { get; set; }
    public int StockActual { get; set; }
    public int StockMinimo { get; set; }
    public string? Descripcion { get; set; }
}
