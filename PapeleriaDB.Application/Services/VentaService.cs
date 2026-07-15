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

    public async Task<IEnumerable<VentaHistorialDto>> GetRecentAsync(int limit)
    {
        var ventas = await _unitOfWork.Ventas.GetRecentWithDetailsAsync(Math.Clamp(limit, 1, 200));
        return ventas.Select(v => new VentaHistorialDto
        {
            Id = v.Id, Folio = v.Folio,
            Fecha = v.Fecha.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(v.Fecha, DateTimeKind.Local) : v.Fecha,
            Subtotal = v.Subtotal, Descuento = v.Descuento, Total = v.Total,
            Estado = v.Estado, MetodoPago = v.MetodoPagoPrincipal,
            Detalles = v.Detalles.Select(d => new VentaHistorialDetalleDto { Id = d.Id, Tipo = d.TipoItem, Descripcion = d.Descripcion, Cantidad = d.Cantidad, PrecioUnitario = d.PrecioUnitario, Subtotal = d.Subtotal })
        });
    }

    public async Task<VentaResponseDto> RegistrarVentaAsync(CrearVentaDto dto)
    {
        try
        {
            var caja = await _unitOfWork.Cajas.GetByIdAsync(dto.CajaId);
            if (caja == null)
            {
                return new VentaResponseDto { Exito = false, Mensaje = "La caja asignada no existe." };
            }

            if (!caja.EstaAbierta)
            {
                return new VentaResponseDto { Exito = false, Mensaje = $"{caja.Nombre} está cerrada. Ábrela antes de registrar ventas." };
            }

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
                if (detalleDto.ProductoId.HasValue)
                {
                    // Es un Producto
                    var producto = await _unitOfWork.Productos.GetByIdAsync(detalleDto.ProductoId.Value);
                    if (producto == null)
                    {
                        return new VentaResponseDto { Exito = false, Mensaje = $"El producto con ID {detalleDto.ProductoId} no existe." };
                    }

                    if (producto.StockActual < detalleDto.Cantidad)
                    {
                        return new VentaResponseDto { Exito = false, Mensaje = $"Stock insuficiente para el producto {producto.Nombre}. Solicitado: {detalleDto.Cantidad}, Disponible: {producto.StockActual}" };
                    }

                    producto.StockActual -= detalleDto.Cantidad;
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
                    await _unitOfWork.Productos.UpdateAsync(producto);
                }
                else if (detalleDto.ServicioId.HasValue)
                {
                    // Es un Servicio
                    var servicio = await _unitOfWork.Servicios.GetByIdAsync(detalleDto.ServicioId.Value);
                    if (servicio == null)
                    {
                        return new VentaResponseDto { Exito = false, Mensaje = $"El servicio con ID {detalleDto.ServicioId} no existe." };
                    }

                    var subtotalItem = detalleDto.Cantidad * servicio.PrecioBase;

                    var detalleVenta = new DetalleVenta
                    {
                        ServicioId = servicio.Id,
                        Cantidad = detalleDto.Cantidad,
                        PrecioUnitario = servicio.PrecioBase,
                        Subtotal = subtotalItem,
                        TipoItem = "Servicio",
                        Descripcion = servicio.Nombre
                    };

                    venta.Detalles.Add(detalleVenta);
                    venta.Subtotal += subtotalItem;
                }
                else
                {
                    return new VentaResponseDto { Exito = false, Mensaje = "El detalle de venta debe especificar un ProductoId o un ServicioId." };
                }
            }

            // Aplicar Descuento y calcular Total final
            venta.Descuento = dto.Descuento;
            venta.Total = venta.Subtotal - venta.Descuento;

            if (venta.Total < 0) venta.Total = 0;

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

    public async Task<DevolucionResponseDto> DevolverArticulosAsync(int ventaId, List<DevolucionItemDto> devoluciones)
    {
        try
        {
            var venta = await _unitOfWork.Ventas.GetVentaConDetallesAsync(ventaId);
            if (venta == null)
            {
                return new DevolucionResponseDto { Exito = false, Mensaje = "Venta no encontrada." };
            }

            if (venta.Estado == "Cancelada")
            {
                return new DevolucionResponseDto { Exito = false, Mensaje = "La venta ya está cancelada." };
            }

            decimal totalReembolso = 0;

            foreach (var devolucion in devoluciones)
            {
                var detalle = venta.Detalles.FirstOrDefault(d => d.Id == devolucion.DetalleVentaId);
                if (detalle == null)
                {
                    return new DevolucionResponseDto { Exito = false, Mensaje = $"Detalle con ID {devolucion.DetalleVentaId} no pertenece a esta venta." };
                }

                if (devolucion.CantidadDevolver <= 0 || devolucion.CantidadDevolver > detalle.Cantidad)
                {
                    return new DevolucionResponseDto { Exito = false, Mensaje = $"Cantidad a devolver inválida para el detalle {devolucion.DetalleVentaId}." };
                }

                // Calcular monto a reembolsar
                decimal reembolsoPorItem = detalle.PrecioUnitario * devolucion.CantidadDevolver;
                totalReembolso += reembolsoPorItem;

                // Actualizar detalle de venta
                detalle.Cantidad -= devolucion.CantidadDevolver;
                detalle.Subtotal -= reembolsoPorItem;

                // Actualizar total de la venta
                venta.Subtotal -= reembolsoPorItem;
                venta.Total -= reembolsoPorItem;

                // Si era producto, restaurar stock
                if (detalle.TipoItem == "Producto" && detalle.ProductoId.HasValue)
                {
                    var producto = await _unitOfWork.Productos.GetByIdAsync(detalle.ProductoId.Value);
                    if (producto != null)
                    {
                        producto.StockActual += devolucion.CantidadDevolver;
                        await _unitOfWork.Productos.UpdateAsync(producto);
                    }
                }
            }

            // Si todos los detalles se quedaron en cantidad 0, marcar la venta como cancelada
            if (venta.Detalles.All(d => d.Cantidad == 0))
            {
                venta.Estado = "Cancelada";
            }

            // Actualizar Venta
            await _unitOfWork.Ventas.UpdateAsync(venta);

            // Registrar el movimiento de egreso en caja por la devolución
            var movimientoCaja = new MovimientoCaja
            {
                CajaId = venta.CajaId,
                Tipo = "Egreso",
                Monto = totalReembolso,
                Motivo = $"Devolución parcial/total de Venta {venta.Folio}",
                Fecha = DateTime.Now,
                UsuarioId = venta.UsuarioId
            };
            
            // Wait, I UnitOfWork doesn't have MovimientosCaja exposed!
            // I should just add the Movimiento to the Caja itself.
            var caja = await _unitOfWork.Cajas.GetByIdAsync(venta.CajaId);
            if (caja != null)
            {
                caja.Movimientos.Add(movimientoCaja);
                await _unitOfWork.Cajas.UpdateAsync(caja);
            }

            await _unitOfWork.CompleteAsync();

            return new DevolucionResponseDto
            {
                Exito = true,
                Mensaje = "Devolución procesada correctamente.",
                MontoReembolsado = totalReembolso
            };
        }
        catch (Exception ex)
        {
            return new DevolucionResponseDto { Exito = false, Mensaje = "Error interno al procesar devolución: " + ex.Message };
        }
    }
}
