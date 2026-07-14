using Moq;
using PapeleriaDB.Application.DTOs;
using PapeleriaDB.Application.Services;
using PapeleriaDB.Domain.Entities;
using PapeleriaDB.Domain.Interfaces;
using Xunit;

namespace PapeleriaDB.Tests;

public class VentaServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IProductoRepository> _mockProductoRepo;
    private readonly Mock<IVentaRepository> _mockVentaRepo;
    private readonly VentaService _ventaService;

    public VentaServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockProductoRepo = new Mock<IProductoRepository>();
        _mockVentaRepo = new Mock<IVentaRepository>();

        _mockUnitOfWork.Setup(u => u.Productos).Returns(_mockProductoRepo.Object);
        _mockUnitOfWork.Setup(u => u.Ventas).Returns(_mockVentaRepo.Object);

        _ventaService = new VentaService(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task RegistrarVenta_Falla_Si_ProductoNoExiste()
    {
        // Arrange
        _mockProductoRepo.Setup(p => p.GetByIdAsync(99)).ReturnsAsync((Producto?)null);

        var dto = new CrearVentaDto
        {
            CajaId = 1,
            UsuarioId = 1,
            Detalles = new List<CrearDetalleVentaDto> { new CrearDetalleVentaDto { ProductoId = 99, Cantidad = 1 } }
        };

        // Act
        var result = await _ventaService.RegistrarVentaAsync(dto);

        // Assert
        Assert.False(result.Exito);
        Assert.Contains("no existe", result.Mensaje);
    }

    [Fact]
    public async Task RegistrarVenta_Falla_Si_NoHayStockSuficiente()
    {
        // Arrange
        var productoExistente = new Producto { Id = 1, Nombre = "Cuaderno", StockActual = 2, PrecioVenta = 10 };
        _mockProductoRepo.Setup(p => p.GetByIdAsync(1)).ReturnsAsync(productoExistente);

        var dto = new CrearVentaDto
        {
            CajaId = 1,
            UsuarioId = 1,
            Detalles = new List<CrearDetalleVentaDto> { new CrearDetalleVentaDto { ProductoId = 1, Cantidad = 5 } }
        };

        // Act
        var result = await _ventaService.RegistrarVentaAsync(dto);

        // Assert
        Assert.False(result.Exito);
        Assert.Contains("Stock insuficiente", result.Mensaje);
    }

    [Fact]
    public async Task RegistrarVenta_Exito_DescuentaStock_Y_CalculaTotal()
    {
        // Arrange
        var productoExistente = new Producto { Id = 1, Nombre = "Cuaderno", StockActual = 10, PrecioVenta = 20 };
        _mockProductoRepo.Setup(p => p.GetByIdAsync(1)).ReturnsAsync(productoExistente);

        var dto = new CrearVentaDto
        {
            CajaId = 1,
            UsuarioId = 1,
            Detalles = new List<CrearDetalleVentaDto> { new CrearDetalleVentaDto { ProductoId = 1, Cantidad = 3 } }
        };

        // Act
        var result = await _ventaService.RegistrarVentaAsync(dto);

        // Assert
        Assert.True(result.Exito);
        Assert.Equal(60, result.Total); // 3 * 20 = 60
        Assert.Equal(7, productoExistente.StockActual); // 10 - 3 = 7

        // Verificamos que los repositorios hayan guardado la info
        _mockVentaRepo.Verify(v => v.AddAsync(It.IsAny<Venta>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
    }
}
