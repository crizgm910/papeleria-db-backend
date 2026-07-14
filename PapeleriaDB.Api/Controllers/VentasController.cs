using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PapeleriaDB.Application.DTOs;
using PapeleriaDB.Application.Services;

namespace PapeleriaDB.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class VentasController : ControllerBase
{
    private readonly IVentaService _ventaService;

    public VentasController(IVentaService ventaService)
    {
        _ventaService = ventaService;
    }

    [HttpPost]
    public async Task<IActionResult> RegistrarVenta([FromBody] CrearVentaDto dto)
    {
        var respuesta = await _ventaService.RegistrarVentaAsync(dto);
        if (!respuesta.Exito)
        {
            return BadRequest(respuesta);
        }

        return Ok(respuesta);
    }

    [HttpGet]
    public async Task<IActionResult> GetRecent([FromQuery] int limit = 50) => Ok(await _ventaService.GetRecentAsync(limit));

    [HttpPost("{id}/devolucion")]
    public async Task<IActionResult> RegistrarDevolucion(int id, [FromBody] List<DevolucionItemDto> devoluciones)
    {
        if (devoluciones == null || !devoluciones.Any())
        {
            return BadRequest("Debe especificar al menos un artículo para devolver.");
        }

        var respuesta = await _ventaService.DevolverArticulosAsync(id, devoluciones);
        if (!respuesta.Exito)
        {
            return BadRequest(respuesta);
        }

        return Ok(respuesta);
    }
}
