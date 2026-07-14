using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PapeleriaDB.Application.DTOs;
using PapeleriaDB.Application.Services;

namespace PapeleriaDB.Api.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class ProveedoresController : ControllerBase
{
    private readonly IProveedorService _service;
    public ProveedoresController(IProveedorService service) => _service = service;
    [HttpGet] public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());
    [HttpPost] public async Task<IActionResult> Create(GuardarProveedorDto dto) => Ok(await _service.CreateAsync(dto));
    [HttpPut("{id:int}")] public async Task<IActionResult> Update(int id, GuardarProveedorDto dto) => Ok(await _service.UpdateAsync(id, dto));
    [HttpDelete("{id:int}")] public async Task<IActionResult> Deactivate(int id) { await _service.DeactivateAsync(id); return NoContent(); }
}
