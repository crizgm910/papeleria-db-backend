using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PapeleriaDB.Application.DTOs;
using PapeleriaDB.Application.Services;

namespace PapeleriaDB.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ServiciosController : ControllerBase
{
    private readonly IServicioService _service;
    public ServiciosController(IServicioService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] GuardarServicioDto dto) => Ok(await _service.CreateAsync(dto));
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] GuardarServicioDto dto) => Ok(await _service.UpdateAsync(id, dto));
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id) { await _service.DeleteAsync(id); return NoContent(); }
}
