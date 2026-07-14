using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PapeleriaDB.Application.DTOs;
using PapeleriaDB.Application.Services;

namespace PapeleriaDB.Api.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class ComprasController : ControllerBase
{
    private readonly ICompraService _service;
    public ComprasController(ICompraService service) => _service = service;
    [HttpGet] public async Task<IActionResult> GetRecent([FromQuery] int limit = 50) => Ok(await _service.GetRecentAsync(limit));
    [HttpGet("{id:int}")] public async Task<IActionResult> GetById(int id) => Ok(await _service.GetByIdAsync(id));
    [HttpPost] public async Task<IActionResult> Register(RegistrarCompraDto dto)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return Ok(await _service.RegisterAsync(dto, userId));
    }
}
