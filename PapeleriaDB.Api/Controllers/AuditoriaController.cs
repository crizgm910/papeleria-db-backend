using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PapeleriaDB.Application.Services;

namespace PapeleriaDB.Api.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class AuditoriaController : ControllerBase
{
    private readonly IAuditoriaService _service;
    public AuditoriaController(IAuditoriaService service) => _service = service;
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int limit = 100, [FromQuery] string? recurso = null) => Ok(await _service.GetRecentAsync(limit, recurso));
}
