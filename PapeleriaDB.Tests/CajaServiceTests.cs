using Moq;
using PapeleriaDB.Application.DTOs;
using PapeleriaDB.Application.Services;
using PapeleriaDB.Domain.Entities;
using PapeleriaDB.Domain.Interfaces;
using Xunit;

namespace PapeleriaDB.Tests;

public class CajaServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<ICajaRepository> _cajas = new();

    [Fact]
    public async Task CreateAsync_CreaUnaCajaSinLimiteFijoDeCantidad()
    {
        _unitOfWork.Setup(u => u.Cajas).Returns(_cajas.Object);
        _cajas.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Caja>
        {
            new() { Id = 1, Nombre = "Caja Principal" },
            new() { Id = 2, Nombre = "Caja 2" },
            new() { Id = 3, Nombre = "Caja 3" }
        });
        _cajas.Setup(r => r.AddAsync(It.IsAny<Caja>()))
            .Callback<Caja>(c => c.Id = 4)
            .ReturnsAsync((Caja c) => c);

        var service = new CajaService(_unitOfWork.Object);
        var result = await service.CreateAsync(new CrearCajaDto { Nombre = "Caja 4" });

        Assert.Equal(4, result.Id);
        Assert.Equal("Caja 4", result.Nombre);
        _cajas.Verify(r => r.AddAsync(It.IsAny<Caja>()), Times.Once);
        _unitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_RechazaNombresDuplicados()
    {
        _unitOfWork.Setup(u => u.Cajas).Returns(_cajas.Object);
        _cajas.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Caja>
        {
            new() { Id = 1, Nombre = "Caja Principal" }
        });

        var service = new CajaService(_unitOfWork.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.CreateAsync(new CrearCajaDto { Nombre = "caja principal" }));
        _unitOfWork.Verify(u => u.CompleteAsync(), Times.Never);
    }

    [Fact]
    public async Task CerrarCajaAsync_CalculaSoloElTurnoAbierto()
    {
        var apertura = new DateTime(2026, 7, 14, 9, 0, 0);
        var corte = new CorteCaja
        {
            Id = 10,
            CajaId = 2,
            UsuarioId = 1,
            FechaApertura = apertura,
            MontoInicial = 100m,
            Estado = "Abierto"
        };
        var caja = new Caja
        {
            Id = 2,
            Nombre = "Caja 2",
            EstaAbierta = true,
            Ventas =
            [
                new Venta { Total = 80m, MetodoPagoPrincipal = "Efectivo", Estado = "Completada", Fecha = apertura.AddMinutes(10) },
                new Venta { Total = 50m, MetodoPagoPrincipal = "Tarjeta", Estado = "Completada", Fecha = apertura.AddMinutes(20) }
            ],
            Movimientos =
            [
                new MovimientoCaja { Tipo = "Ingreso", Monto = 20m, Fecha = apertura.AddMinutes(30) },
                new MovimientoCaja { Tipo = "Egreso", Monto = 10m, Fecha = apertura.AddMinutes(40) }
            ]
        };

        _unitOfWork.SetupGet(u => u.Cajas).Returns(_cajas.Object);
        _cajas.Setup(r => r.GetUltimoCorteAbiertoAsync(2)).ReturnsAsync(corte);
        _cajas.Setup(r => r.GetCajaConActividadDesdeAsync(2, apertura)).ReturnsAsync(caja);

        var service = new CajaService(_unitOfWork.Object);
        var result = await service.CerrarCajaAsync(new CerrarCajaDto
        {
            CajaId = 2,
            UsuarioId = 1,
            EfectivoContado = 185m
        });

        Assert.True(result.Exito);
        Assert.Equal(190m, result.EfectivoEsperado);
        Assert.Equal(-5m, result.Diferencia);
        Assert.Equal(50m, result.TotalVendidoTarjeta);
        Assert.False(caja.EstaAbierta);
        Assert.Equal("Cerrado", corte.Estado);
        _unitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
    }
}
