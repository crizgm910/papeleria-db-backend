using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PapeleriaDB.Application.Services;

namespace PapeleriaDB.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MobileController : ControllerBase
{
    private readonly IMobileDashboardService _dashboardService;

    public MobileController(IMobileDashboardService dashboardService) => _dashboardService = dashboardService;

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        return Ok(await _dashboardService.GetAsync());
    }
}
