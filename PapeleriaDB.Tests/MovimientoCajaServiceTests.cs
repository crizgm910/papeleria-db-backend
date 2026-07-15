using Moq;
using PapeleriaDB.Application.DTOs;
using PapeleriaDB.Application.Services;
using PapeleriaDB.Domain.Entities;
using PapeleriaDB.Domain.Interfaces;
using Xunit;

namespace PapeleriaDB.Tests;

public class MovimientoCajaServiceTests
{
    [Fact]
    public async Task RegistrarAsync_AgregaMovimientoCuandoCajaEstaAbierta()
    {
        var uow = new Mock<IUnitOfWork>();
        var cajas = new Mock<ICajaRepository>();
        var caja = new Caja { Id = 2, Nombre = "Caja 2", EstaAbierta = true };
        uow.SetupGet(x => x.Cajas).Returns(cajas.Object);
        cajas.Setup(x => x.GetByIdAsync(2)).ReturnsAsync(caja);
        var service = new MovimientoCajaService(uow.Object);

        var result = await service.RegistrarAsync(new RegistrarMovimientoCajaDto
        {
            CajaId = 2, Tipo = "Egreso", Monto = 50m, Motivo = "Compra urgente"
        }, 1);

        Assert.Equal("Egreso", result.Tipo);
        Assert.Equal(50m, result.Monto);
        Assert.Single(caja.Movimientos);
        uow.Verify(x => x.CompleteAsync(), Times.Once);
    }

    [Fact]
    public async Task RegistrarAsync_RechazaCajaCerrada()
    {
        var uow = new Mock<IUnitOfWork>();
        var cajas = new Mock<ICajaRepository>();
        uow.SetupGet(x => x.Cajas).Returns(cajas.Object);
        cajas.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new Caja { Id = 1, EstaAbierta = false });
        var service = new MovimientoCajaService(uow.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.RegistrarAsync(
            new RegistrarMovimientoCajaDto { CajaId = 1, Tipo = "Ingreso", Monto = 10m, Motivo = "Cambio" }, 1));
        uow.Verify(x => x.CompleteAsync(), Times.Never);
    }
}
