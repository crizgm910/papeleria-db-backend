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
        var productos = await _reportesService.GetProductosConBajoStockAsync(limite);
        return Ok(productos);
    }

    [HttpGet("top-vendidos")]
    public async Task<IActionResult> GetTopVendidos([FromQuery] int top = 5)
    {
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

        var resumen = await _reportesService.GetResumenVentasPorFechaAsync(inicio, fin);
        return Ok(resumen);
    }
}
