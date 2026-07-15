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
}
