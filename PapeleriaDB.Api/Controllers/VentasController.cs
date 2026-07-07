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
        var response = await _ventaService.RegistrarVentaAsync(dto);
        
        if (!response.Exito)
        {
            return BadRequest(response.Mensaje);
        }

        return Ok(response);
    }
}
