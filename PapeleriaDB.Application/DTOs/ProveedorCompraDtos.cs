namespace PapeleriaDB.Application.DTOs;

public class ProveedorDto
{
    public int Id { get; set; }
    public string NombreEmpresa { get; set; } = string.Empty;
    public string? Contacto { get; set; }
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public string Rfc { get; set; } = string.Empty;
    public bool Activo { get; set; }
}

public class GuardarProveedorDto
{
    public string NombreEmpresa { get; set; } = string.Empty;
    public string? Contacto { get; set; }
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public string Rfc { get; set; } = string.Empty;
}

public class RegistrarCompraDto
{
    public int ProveedorId { get; set; }
    public string? FolioProveedor { get; set; }
    public string? Notas { get; set; }
    public List<RegistrarDetalleCompraDto> Detalles { get; set; } = [];
}

public class RegistrarDetalleCompraDto
{
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
    public decimal CostoUnitario { get; set; }
}

public class CompraDto
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public int ProveedorId { get; set; }
    public string Proveedor { get; set; } = string.Empty;
    public string FolioProveedor { get; set; } = string.Empty;
    public string? Notas { get; set; }
    public decimal Total { get; set; }
    public int TotalArticulos { get; set; }
    public List<DetalleCompraDto> Detalles { get; set; } = [];
}

public class DetalleCompraDto
{
    public int ProductoId { get; set; }
    public string Producto { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal CostoUnitario { get; set; }
    public decimal Subtotal { get; set; }
}
