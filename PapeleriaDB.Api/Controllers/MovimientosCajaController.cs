using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PapeleriaDB.Application.DTOs;
using PapeleriaDB.Application.Services;

namespace PapeleriaDB.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/movimientos-caja")]
public class MovimientosCajaController : ControllerBase
{
    private readonly IMovimientoCajaService _service;
    public MovimientosCajaController(IMovimientoCajaService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int cajaId, [FromQuery] int limit = 50) =>
        Ok(await _service.GetRecentAsync(cajaId, limit));

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegistrarMovimientoCajaDto dto)
    {
        var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return Ok(await _service.RegistrarAsync(dto, usuarioId));
    }
}
