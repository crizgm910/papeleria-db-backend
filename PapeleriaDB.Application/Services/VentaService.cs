using PapeleriaDB.Application.DTOs;
using PapeleriaDB.Domain.Entities;
using PapeleriaDB.Domain.Interfaces;

namespace PapeleriaDB.Application.Services;

public class VentaService : IVentaService
{
    private readonly IUnitOfWork _unitOfWork;

    public VentaService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<VentaResponseDto> RegistrarVentaAsync(CrearVentaDto dto)
    {
        try
        {
            var venta = new Venta
            {
                Folio = $"V-{DateTime.Now:yyyyMMddHHmmss}",
                Fecha = DateTime.Now,
                CajaId = dto.CajaId,
                UsuarioId = dto.UsuarioId,
                MetodoPagoPrincipal = dto.MetodoPagoPrincipal,
                Estado = "Completada",
                Subtotal = 0,
                Total = 0
            };

            foreach (var detalleDto in dto.Detalles)
            {
                // Validación 1: ¿El producto existe?
                var producto = await _unitOfWork.Productos.GetByIdAsync(detalleDto.ProductoId);
                if (producto == null)
                {
                    return new VentaResponseDto { Exito = false, Mensaje = $"El producto con ID {detalleDto.ProductoId} no existe." };
                }

                // Validación 2: ¿Hay stock suficiente?
                if (producto.StockActual < detalleDto.Cantidad)
                {
                    return new VentaResponseDto { Exito = false, Mensaje = $"Stock insuficiente para el producto {producto.Nombre}. Solicitado: {detalleDto.Cantidad}, Disponible: {producto.StockActual}" };
                }

                // Descuento de inventario en tiempo real
                producto.StockActual -= detalleDto.Cantidad;

                // Cálculos matemáticos
                var subtotalItem = detalleDto.Cantidad * producto.PrecioVenta;
                
                var detalleVenta = new DetalleVenta
                {
                    ProductoId = producto.Id,
                    Cantidad = detalleDto.Cantidad,
                    PrecioUnitario = producto.PrecioVenta,
                    Subtotal = subtotalItem,
                    TipoItem = "Producto",
                    Descripcion = producto.Nombre
                };

                venta.Detalles.Add(detalleVenta);
                venta.Subtotal += subtotalItem;
                venta.Total += subtotalItem;
                
                // Actualizamos el producto en memoria (Unit of work lo rastreará)
                await _unitOfWork.Productos.UpdateAsync(producto);
            }

            // Agregamos la venta completa al repositorio
            await _unitOfWork.Ventas.AddAsync(venta);
            
            // CONFIRMACIÓN ATÓMICA: Se guarda la venta, los detalles y el descuento de stock al mismo tiempo.
            await _unitOfWork.CompleteAsync();

            return new VentaResponseDto
            {
                Exito = true,
                Id = venta.Id,
                Folio = venta.Folio,
                Total = venta.Total,
                Estado = venta.Estado,
                Mensaje = "Venta registrada exitosamente."
            };
        }
        catch (Exception ex)
        {
            return new VentaResponseDto { Exito = false, Mensaje = "Error interno al procesar la venta: " + ex.Message };
        }
    }
}
