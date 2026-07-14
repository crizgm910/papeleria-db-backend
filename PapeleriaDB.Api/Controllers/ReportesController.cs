using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PapeleriaDB.Application.Services;

namespace PapeleriaDB.Api.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class ReportesController : ControllerBase
{
    private readonly IReportesService _reportesService;

    public ReportesController(IReportesService reportesService)
    {
        _reportesService = reportesService;
    }

    [HttpGet("bajo-stock")]
    public async Task<IActionResult> GetBajoStock([FromQuery] int limite = 5)
    {
        if (limite < 0) return BadRequest("El límite no puede ser negativo.");
        var productos = await _reportesService.GetProductosConBajoStockAsync(limite);
        return Ok(productos);
    }

    [HttpGet("top-vendidos")]
    public async Task<IActionResult> GetTopVendidos([FromQuery] int top = 5)
    {
        if (top < 1 || top > 100) return BadRequest("Top debe estar entre 1 y 100.");
        var topProductos = await _reportesService.GetTopProductosMasVendidosAsync(top);
        return Ok(topProductos);
    }

    [HttpGet("ventas-por-fecha")]
    public async Task<IActionResult> GetVentasPorFecha([FromQuery] DateTime inicio, [FromQuery] DateTime fin)
    {
        if (inicio == default || fin == default)
        {
            return BadRequest("Debe proporcionar las fechas de 'inicio' y 'fin'.");
        }
        if (fin.Date < inicio.Date) return BadRequest("La fecha final no puede ser anterior a la inicial.");

        var resumen = await _reportesService.GetResumenVentasPorFechaAsync(inicio, fin);
        return Ok(resumen);
    }
}
