using Moq;
using PapeleriaDB.Application.DTOs;
using PapeleriaDB.Application.Services;
using PapeleriaDB.Domain.Entities;
using PapeleriaDB.Domain.Interfaces;
using Xunit;

namespace PapeleriaDB.Tests;

public class VentaCajaTests
{
    [Fact]
    public async Task RegistrarVentaAsync_RechazaCajaCerrada()
    {
        var unitOfWork = new Mock<IUnitOfWork>();
        var cajas = new Mock<ICajaRepository>();
        var ventas = new Mock<IVentaRepository>();
        unitOfWork.SetupGet(u => u.Cajas).Returns(cajas.Object);
        unitOfWork.SetupGet(u => u.Ventas).Returns(ventas.Object);
        cajas.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(new Caja
        {
            Id = 2,
            Nombre = "Caja 2",
            EstaAbierta = false
        });

        var service = new VentaService(unitOfWork.Object);
        var result = await service.RegistrarVentaAsync(new CrearVentaDto
        {
            CajaId = 2,
            UsuarioId = 1,
            Detalles = [new CrearDetalleVentaDto { ProductoId = 1, Cantidad = 1 }]
        });

        Assert.False(result.Exito);
        Assert.Contains("cerrada", result.Mensaje, StringComparison.OrdinalIgnoreCase);
        ventas.Verify(r => r.AddAsync(It.IsAny<Venta>()), Times.Never);
        unitOfWork.Verify(u => u.CompleteAsync(), Times.Never);
    }

    [Fact]
    public async Task DevolverArticulosAsync_RechazaCajaCerradaSinModificarVenta()
    {
        var unitOfWork = new Mock<IUnitOfWork>();
        var cajas = new Mock<ICajaRepository>();
        var ventas = new Mock<IVentaRepository>();
        var venta = new Venta
        {
            Id = 1, CajaId = 1, Folio = "V-TEST", Total = 35m, Subtotal = 35m, Estado = "Completada",
            Detalles = [new DetalleVenta { Id = 8, Cantidad = 1, PrecioUnitario = 35m, Subtotal = 35m, TipoItem = "Producto", ProductoId = 1 }]
        };
        unitOfWork.SetupGet(u => u.Cajas).Returns(cajas.Object);
        unitOfWork.SetupGet(u => u.Ventas).Returns(ventas.Object);
        ventas.Setup(r => r.GetVentaConDetallesAsync(1)).ReturnsAsync(venta);
        cajas.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Caja { Id = 1, EstaAbierta = false });

        var service = new VentaService(unitOfWork.Object);
        var result = await service.DevolverArticulosAsync(1, [new DevolucionItemDto { DetalleVentaId = 8, CantidadDevolver = 1 }]);

        Assert.False(result.Exito);
        Assert.Contains("cerrada", result.Mensaje, StringComparison.OrdinalIgnoreCase);
        Assert.Equal(35m, venta.Total);
        Assert.Equal(1, venta.Detalles.Single().Cantidad);
        unitOfWork.Verify(u => u.CompleteAsync(), Times.Never);
    }
}
