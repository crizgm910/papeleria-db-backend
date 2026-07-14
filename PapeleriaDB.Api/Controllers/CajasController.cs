using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PapeleriaDB.Application.DTOs;
using PapeleriaDB.Application.Services;

namespace PapeleriaDB.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CajasController : ControllerBase
{
    private readonly ICajaService _cajaService;

    public CajasController(ICajaService cajaService)
    {
        _cajaService = cajaService;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("supervision")]
    public async Task<IActionResult> GetSupervision([FromQuery] int historial = 20)
    {
        return Ok(await _cajaService.GetSupervisionAsync(historial));
    }

    [HttpPost("abrir")]
    public async Task<IActionResult> AbrirCaja([FromBody] AbrirCajaDto dto)
    {
        var response = await _cajaService.AbrirCajaAsync(dto);
        
        if (!response.Exito)
        {
            return BadRequest(response.Mensaje);
        }

        return Ok(response);
    }

    [HttpPost("cerrar")]
    public async Task<IActionResult> CerrarCaja([FromBody] CerrarCajaDto dto)
    {
        var response = await _cajaService.CerrarCajaAsync(dto);
        
        if (!response.Exito)
        {
            return BadRequest(response.Mensaje);
        }

        return Ok(response);
    }
}
