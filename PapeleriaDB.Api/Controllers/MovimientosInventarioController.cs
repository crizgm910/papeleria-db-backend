using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PapeleriaDB.Application.DTOs;
using PapeleriaDB.Application.Services;

namespace PapeleriaDB.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/movimientos-inventario")]
public class MovimientosInventarioController : ControllerBase
{
    private readonly IMovimientoInventarioService _service;
    public MovimientosInventarioController(IMovimientoInventarioService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetRecent([FromQuery] int limit = 50) => Ok(await _service.GetRecentAsync(limit));

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegistrarMovimientoInventarioDto dto)
    {
        var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return Ok(await _service.RegisterAsync(dto, usuarioId));
    }
}
