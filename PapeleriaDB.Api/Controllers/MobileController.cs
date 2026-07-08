using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PapeleriaDB.Application.Services;

namespace PapeleriaDB.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MobileController : ControllerBase
{
    private readonly IReportesService _reportesService;
    private readonly ICajaService _cajaService;

    public MobileController(IReportesService reportesService, ICajaService cajaService)
    {
        _reportesService = reportesService;
        _cajaService = cajaService;
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        // Resumen ligero para la app móvil
        var ventasHoy = await _reportesService.GetResumenVentasPorFechaAsync(DateTime.Now, DateTime.Now);
        var alertasStock = await _reportesService.GetProductosConBajoStockAsync(5);
        
        // Retornar un objeto compacto
        return Ok(new
        {
            Fecha = DateTime.Now,
            VentasHoy = ventasHoy,
            TotalAlertasStock = alertasStock.Count()
        });
    }
}
