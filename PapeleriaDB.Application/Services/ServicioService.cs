using PapeleriaDB.Application.DTOs;
using PapeleriaDB.Domain.Entities;
using PapeleriaDB.Domain.Interfaces;

namespace PapeleriaDB.Application.Services;

public class ServicioService : IServicioService
{
    private readonly IUnitOfWork _unitOfWork;
    public ServicioService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<IEnumerable<ServicioDto>> GetAllAsync() =>
        (await _unitOfWork.Servicios.GetAllAsync()).Where(s => s.Estado).OrderBy(s => s.Nombre).Select(Map);

    public async Task<ServicioDto> CreateAsync(GuardarServicioDto dto)
    {
        var servicio = new Servicio
        {
            CodigoInterno = string.IsNullOrWhiteSpace(dto.CodigoInterno) ? $"SRV-{Guid.NewGuid():N}" : dto.CodigoInterno.Trim(),
            Nombre = dto.Nombre.Trim(), PrecioBase = dto.PrecioBase,
            Descripcion = string.IsNullOrWhiteSpace(dto.Descripcion) ? null : dto.Descripcion.Trim()
            ,Estado = true
        };
        await _unitOfWork.Servicios.AddAsync(servicio);
        await _unitOfWork.CompleteAsync();
        return Map(servicio);
    }

    public async Task<ServicioDto> UpdateAsync(int id, GuardarServicioDto dto)
    {
        var servicio = await _unitOfWork.Servicios.GetByIdAsync(id) ?? throw new KeyNotFoundException("El servicio no existe.");
        if (!string.IsNullOrWhiteSpace(dto.CodigoInterno)) servicio.CodigoInterno = dto.CodigoInterno.Trim();
        servicio.Nombre = dto.Nombre.Trim(); servicio.PrecioBase = dto.PrecioBase;
        servicio.Descripcion = string.IsNullOrWhiteSpace(dto.Descripcion) ? null : dto.Descripcion.Trim();
        await _unitOfWork.Servicios.UpdateAsync(servicio);
        await _unitOfWork.CompleteAsync();
        return Map(servicio);
    }

    public async Task DeleteAsync(int id)
    {
        var servicio = await _unitOfWork.Servicios.GetByIdAsync(id) ?? throw new KeyNotFoundException("El servicio no existe.");
        servicio.Estado = false;
        await _unitOfWork.Servicios.UpdateAsync(servicio);
        await _unitOfWork.CompleteAsync();
    }

    private static ServicioDto Map(Servicio s) => new() { Id = s.Id, CodigoInterno = s.CodigoInterno, Nombre = s.Nombre, PrecioBase = s.PrecioBase, Descripcion = s.Descripcion, Estado = s.Estado };
}
