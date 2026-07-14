using PapeleriaDB.Application.DTOs;
using PapeleriaDB.Domain.Entities;
using PapeleriaDB.Domain.Interfaces;

namespace PapeleriaDB.Application.Services;

public class CompraService : ICompraService
{
    private readonly IUnitOfWork _unitOfWork;
    public CompraService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<IEnumerable<CompraDto>> GetRecentAsync(int limit) =>
        (await _unitOfWork.Compras.GetRecentAsync(Math.Clamp(limit, 1, 200))).Select(Map);

    public async Task<CompraDto> GetByIdAsync(int id) => Map(
        await _unitOfWork.Compras.GetDetailAsync(id) ?? throw new KeyNotFoundException("La compra no existe."));

    public async Task<CompraDto> RegisterAsync(RegistrarCompraDto dto, int usuarioId)
    {
        var proveedor = await _unitOfWork.Proveedores.GetByIdAsync(dto.ProveedorId)
            ?? throw new KeyNotFoundException("El proveedor no existe.");
        if (!proveedor.Activo) throw new InvalidOperationException("El proveedor está inactivo.");
        if (dto.Detalles.GroupBy(d => d.ProductoId).Any(g => g.Count() > 1))
            throw new ArgumentException("Un producto no puede aparecer dos veces en la misma compra.");

        var compra = new Compra { ProveedorId = proveedor.Id, Proveedor = proveedor, UsuarioId = usuarioId, Fecha = DateTime.UtcNow, FolioProveedor = dto.FolioProveedor?.Trim() ?? string.Empty, Notas = dto.Notas?.Trim() };
        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            foreach (var item in dto.Detalles)
            {
                var producto = await _unitOfWork.Productos.GetByIdAsync(item.ProductoId)
                    ?? throw new KeyNotFoundException($"El producto {item.ProductoId} no existe.");
                var anterior = producto.StockActual;
                producto.StockActual += item.Cantidad;
                producto.CostoCompra = item.CostoUnitario;
                var subtotal = item.Cantidad * item.CostoUnitario;
                compra.Detalles.Add(new DetalleCompra { ProductoId = producto.Id, Producto = producto, Cantidad = item.Cantidad, CostoUnitario = item.CostoUnitario, Subtotal = subtotal });
                compra.Total += subtotal;
                await _unitOfWork.MovimientosInventario.AddAsync(new MovimientoInventario { ProductoId = producto.Id, Producto = producto, Tipo = "Entrada", Cantidad = item.Cantidad, StockAnterior = anterior, StockNuevo = producto.StockActual, Motivo = $"Compra a {proveedor.NombreEmpresa}", UsuarioId = usuarioId, Fecha = compra.Fecha });
            }
            await _unitOfWork.Compras.AddAsync(compra);
        });
        return Map(compra);
    }

    private static CompraDto Map(Compra c) => new()
    {
        Id = c.Id, Fecha = c.Fecha.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(c.Fecha, DateTimeKind.Utc) : c.Fecha.ToUniversalTime(),
        ProveedorId = c.ProveedorId, Proveedor = c.Proveedor?.NombreEmpresa ?? "Proveedor", FolioProveedor = c.FolioProveedor,
        Notas = c.Notas, Total = c.Total, TotalArticulos = c.Detalles.Sum(d => d.Cantidad),
        Detalles = c.Detalles.Select(d => new DetalleCompraDto { ProductoId = d.ProductoId, Producto = d.Producto?.Nombre ?? "Producto", Cantidad = d.Cantidad, CostoUnitario = d.CostoUnitario, Subtotal = d.Subtotal }).ToList()
    };
}
