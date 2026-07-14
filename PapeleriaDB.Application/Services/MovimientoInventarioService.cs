using PapeleriaDB.Application.DTOs;
using PapeleriaDB.Domain.Entities;
using PapeleriaDB.Domain.Interfaces;

namespace PapeleriaDB.Application.Services;

public class MovimientoInventarioService : IMovimientoInventarioService
{
    private readonly IUnitOfWork _unitOfWork;
    public MovimientoInventarioService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<IEnumerable<MovimientoInventarioDto>> GetRecentAsync(int limit) =>
        (await _unitOfWork.MovimientosInventario.GetRecentAsync(Math.Clamp(limit, 1, 200))).Select(Map);

    public async Task<MovimientoInventarioDto> RegisterAsync(RegistrarMovimientoInventarioDto dto, int usuarioId)
    {
        var producto = await _unitOfWork.Productos.GetByIdAsync(dto.ProductoId)
            ?? throw new KeyNotFoundException("El producto no existe.");
        if (dto.Tipo == "Entrada" && dto.Cantidad < 0)
            throw new ArgumentException("Una entrada debe tener cantidad positiva.");

        var nuevoStock = producto.StockActual + dto.Cantidad;
        if (nuevoStock < 0) throw new InvalidOperationException("El ajuste dejaría el inventario en negativo.");

        var movimiento = new MovimientoInventario
        {
            ProductoId = producto.Id, Producto = producto, Tipo = dto.Tipo,
            Cantidad = dto.Cantidad, StockAnterior = producto.StockActual, StockNuevo = nuevoStock,
            Motivo = dto.Motivo.Trim(), UsuarioId = usuarioId, Fecha = DateTime.UtcNow
        };
        producto.StockActual = nuevoStock;
        await _unitOfWork.Productos.UpdateAsync(producto);
        await _unitOfWork.MovimientosInventario.AddAsync(movimiento);
        await _unitOfWork.CompleteAsync();
        return Map(movimiento);
    }

    private static MovimientoInventarioDto Map(MovimientoInventario m) => new()
    {
        Id = m.Id, ProductoId = m.ProductoId, ProductoNombre = m.Producto?.Nombre ?? "Producto",
        Tipo = m.Tipo, Cantidad = m.Cantidad, StockAnterior = m.StockAnterior,
        StockNuevo = m.StockNuevo, Motivo = m.Motivo,
        Fecha = m.Fecha.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(m.Fecha, DateTimeKind.Utc)
            : m.Fecha.ToUniversalTime()
    };
}
