using PapeleriaDB.Application.DTOs;
using PapeleriaDB.Domain.Entities;
using PapeleriaDB.Domain.Interfaces;

namespace PapeleriaDB.Application.Services;

public class ProductoService : IProductoService
{
    private readonly IUnitOfWork _unitOfWork;

    public ProductoService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<ProductoDto>> GetAllAsync()
    {
        var productos = await _unitOfWork.Productos.GetAllAsync();
        return productos.Select(p => new ProductoDto
        {
            Id = p.Id,
            CodigoBarras = p.CodigoBarras ?? "",
            Nombre = p.Nombre,
            PrecioVenta = p.PrecioVenta,
            StockActual = p.StockActual
        });
    }

    public async Task<ProductoDto?> GetByIdAsync(int id)
    {
        var producto = await _unitOfWork.Productos.GetByIdAsync(id);
        if (producto == null) return null;

        return new ProductoDto
        {
            Id = producto.Id,
            CodigoBarras = producto.CodigoBarras ?? "",
            Nombre = producto.Nombre,
            PrecioVenta = producto.PrecioVenta,
            StockActual = producto.StockActual
        };
    }

    public async Task<ProductoDto?> GetByCodigoBarrasAsync(string codigoBarras)
    {
        var producto = await _unitOfWork.Productos.GetByCodigoBarrasAsync(codigoBarras);
        if (producto == null) return null;

        return new ProductoDto
        {
            Id = producto.Id,
            CodigoBarras = producto.CodigoBarras ?? "",
            Nombre = producto.Nombre,
            PrecioVenta = producto.PrecioVenta,
            StockActual = producto.StockActual
        };
    }

    public async Task<ProductoDto> CreateAsync(CrearProductoDto dto)
    {
        var nuevoProducto = new Producto
        {
            CodigoInterno = dto.CodigoInterno,
            CodigoBarras = dto.CodigoBarras,
            Nombre = dto.Nombre,
            CategoriaId = dto.CategoriaId,
            PrecioVenta = dto.PrecioVenta,
            CostoCompra = dto.CostoCompra,
            StockActual = dto.StockActual,
            StockMinimo = dto.StockMinimo,
            Descripcion = dto.Descripcion,
            Estado = true
        };

        await _unitOfWork.Productos.AddAsync(nuevoProducto);
        await _unitOfWork.CompleteAsync();

        return new ProductoDto
        {
            Id = nuevoProducto.Id,
            CodigoBarras = nuevoProducto.CodigoBarras ?? "",
            Nombre = nuevoProducto.Nombre,
            PrecioVenta = nuevoProducto.PrecioVenta,
            StockActual = nuevoProducto.StockActual
        };
    }
}
