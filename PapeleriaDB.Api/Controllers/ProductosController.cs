using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PapeleriaDB.Application.DTOs;
using PapeleriaDB.Application.Services;

namespace PapeleriaDB.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    private readonly IProductoService _productoService;

    public ProductosController(IProductoService productoService)
    {
        _productoService = productoService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var productos = await _productoService.GetAllAsync();
        return Ok(productos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var producto = await _productoService.GetByIdAsync(id);
        if (producto == null) return NotFound();
        return Ok(producto);
    }

    [HttpGet("codigo/{codigoBarras}")]
    public async Task<IActionResult> GetByCodigoBarras(string codigoBarras)
    {
        var producto = await _productoService.GetByCodigoBarrasAsync(codigoBarras);
        if (producto == null) return NotFound();
        return Ok(producto);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CrearProductoDto dto)
    {
        var nuevoProducto = await _productoService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = nuevoProducto.Id }, nuevoProducto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CrearProductoDto dto)
    {
        return Ok(await _productoService.UpdateAsync(id, dto));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _productoService.DeleteAsync(id);
        return NoContent();
    }
}
