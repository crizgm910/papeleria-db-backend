using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PapeleriaDB.Application.DTOs;
using PapeleriaDB.Application.Services;

namespace PapeleriaDB.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CategoriasController : ControllerBase
{
    private readonly ICategoriaService _categoriaService;

    public CategoriasController(ICategoriaService categoriaService)
    {
        _categoriaService = categoriaService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _categoriaService.GetAllAsync());
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CrearCategoriaDto dto)
    {
        var categoria = await _categoriaService.CreateAsync(dto);
        return Created($"api/Categorias/{categoria.Id}", categoria);
    }
}
