using PapeleriaDB.Application.DTOs;
using PapeleriaDB.Domain.Entities;
using PapeleriaDB.Domain.Interfaces;
using System.Security.Cryptography;

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
            CodigoInterno = p.CodigoInterno,
            CodigoBarras = p.CodigoBarras ?? "",
            Nombre = p.Nombre,
            CategoriaId = p.CategoriaId,
            PrecioVenta = p.PrecioVenta,
            CostoCompra = p.CostoCompra,
            StockActual = p.StockActual,
            StockMinimo = p.StockMinimo,
            Descripcion = p.Descripcion
        });
    }

    public async Task<ProductoDto?> GetByIdAsync(int id)
    {
        var producto = await _unitOfWork.Productos.GetByIdAsync(id);
        if (producto == null) return null;

        return new ProductoDto
        {
            Id = producto.Id,
            CodigoInterno = producto.CodigoInterno,
            CodigoBarras = producto.CodigoBarras ?? "",
            Nombre = producto.Nombre,
            CategoriaId = producto.CategoriaId,
            PrecioVenta = producto.PrecioVenta,
            CostoCompra = producto.CostoCompra,
            StockActual = producto.StockActual,
            StockMinimo = producto.StockMinimo,
            Descripcion = producto.Descripcion
        };
    }

    public async Task<ProductoDto?> GetByCodigoBarrasAsync(string codigoBarras)
    {
        var producto = await _unitOfWork.Productos.GetByCodigoBarrasAsync(codigoBarras);
        if (producto == null) return null;

        return new ProductoDto
        {
            Id = producto.Id,
            CodigoInterno = producto.CodigoInterno,
            CodigoBarras = producto.CodigoBarras ?? "",
            Nombre = producto.Nombre,
            CategoriaId = producto.CategoriaId,
            PrecioVenta = producto.PrecioVenta,
            CostoCompra = producto.CostoCompra,
            StockActual = producto.StockActual,
            StockMinimo = producto.StockMinimo,
            Descripcion = producto.Descripcion
        };
    }

    public async Task<ProductoDto> CreateAsync(CrearProductoDto dto)
    {
        var codigoInterno = string.IsNullOrWhiteSpace(dto.CodigoInterno)
            ? $"AUTO-{Guid.NewGuid():N}"
            : dto.CodigoInterno.Trim();
        var codigoBarras = string.IsNullOrWhiteSpace(dto.CodigoBarras)
            ? GenerateEan13()
            : dto.CodigoBarras.Trim();

        if (await _unitOfWork.Productos.GetByCodigoInternoAsync(codigoInterno) != null)
            throw new InvalidOperationException("Ya existe un producto con ese código interno.");
        if (await _unitOfWork.Productos.GetByCodigoBarrasAsync(codigoBarras) != null)
            throw new InvalidOperationException("Ya existe un producto con ese código de barras.");

        var nuevoProducto = new Producto
        {
            CodigoInterno = codigoInterno,
            CodigoBarras = codigoBarras,
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
            CodigoInterno = nuevoProducto.CodigoInterno,
            CodigoBarras = nuevoProducto.CodigoBarras ?? "",
            Nombre = nuevoProducto.Nombre,
            CategoriaId = nuevoProducto.CategoriaId,
            PrecioVenta = nuevoProducto.PrecioVenta,
            CostoCompra = nuevoProducto.CostoCompra,
            StockActual = nuevoProducto.StockActual,
            StockMinimo = nuevoProducto.StockMinimo,
            Descripcion = nuevoProducto.Descripcion
        };
    }

    public async Task<ProductoDto> UpdateAsync(int id, CrearProductoDto dto)
    {
        var producto = await _unitOfWork.Productos.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("El producto no existe.");

        var codigoInterno = string.IsNullOrWhiteSpace(dto.CodigoInterno) ? producto.CodigoInterno : dto.CodigoInterno.Trim();
        var codigoBarras = string.IsNullOrWhiteSpace(dto.CodigoBarras) ? producto.CodigoBarras! : dto.CodigoBarras.Trim();
        var productoConCodigoInterno = await _unitOfWork.Productos.GetByCodigoInternoAsync(codigoInterno);
        var productoConCodigoBarras = await _unitOfWork.Productos.GetByCodigoBarrasAsync(codigoBarras);
        if (productoConCodigoInterno != null && productoConCodigoInterno.Id != id)
            throw new InvalidOperationException("Ya existe un producto con ese código interno.");
        if (productoConCodigoBarras != null && productoConCodigoBarras.Id != id)
            throw new InvalidOperationException("Ya existe un producto con ese código de barras.");

        producto.CodigoInterno = codigoInterno;
        producto.CodigoBarras = codigoBarras;
        producto.Nombre = dto.Nombre.Trim();
        producto.CategoriaId = dto.CategoriaId;
        producto.PrecioVenta = dto.PrecioVenta;
        producto.CostoCompra = dto.CostoCompra;
        producto.StockActual = dto.StockActual;
        producto.StockMinimo = dto.StockMinimo;
        producto.Descripcion = string.IsNullOrWhiteSpace(dto.Descripcion) ? null : dto.Descripcion.Trim();

        await _unitOfWork.Productos.UpdateAsync(producto);
        await _unitOfWork.CompleteAsync();

        return new ProductoDto
        {
            Id = producto.Id,
            CodigoInterno = producto.CodigoInterno,
            CodigoBarras = producto.CodigoBarras ?? "",
            Nombre = producto.Nombre,
            CategoriaId = producto.CategoriaId,
            PrecioVenta = producto.PrecioVenta,
            CostoCompra = producto.CostoCompra,
            StockActual = producto.StockActual,
            StockMinimo = producto.StockMinimo,
            Descripcion = producto.Descripcion
        };
    }

    public async Task DeleteAsync(int id)
    {
        var producto = await _unitOfWork.Productos.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("El producto no existe.");

        await _unitOfWork.Productos.DeleteAsync(producto);
        await _unitOfWork.CompleteAsync();
    }

    private static string GenerateEan13()
    {
        Span<byte> digits = stackalloc byte[12];
        RandomNumberGenerator.Fill(digits);
        var characters = new char[13];
        var sum = 0;

        for (var index = 0; index < 12; index++)
        {
            var digit = digits[index] % 10;
            characters[index] = (char)('0' + digit);
            sum += digit * (index % 2 == 0 ? 1 : 3);
        }

        characters[12] = (char)('0' + ((10 - sum % 10) % 10));
        return new string(characters);
    }
}
