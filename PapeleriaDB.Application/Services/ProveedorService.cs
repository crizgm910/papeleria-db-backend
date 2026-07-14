using PapeleriaDB.Application.DTOs;
using PapeleriaDB.Domain.Entities;
using PapeleriaDB.Domain.Interfaces;

namespace PapeleriaDB.Application.Services;

public class ProveedorService : IProveedorService
{
    private readonly IUnitOfWork _unitOfWork;
    public ProveedorService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<IEnumerable<ProveedorDto>> GetAllAsync() =>
        (await _unitOfWork.Proveedores.GetAllAsync()).OrderBy(p => p.NombreEmpresa).Select(Map);

    public async Task<ProveedorDto> CreateAsync(GuardarProveedorDto dto)
    {
        var rfc = NormalizeRfc(dto.Rfc);
        if (await _unitOfWork.Proveedores.GetByRfcAsync(rfc) is not null)
            throw new InvalidOperationException("Ya existe un proveedor con ese RFC.");
        var entity = new Proveedor();
        Apply(entity, dto, rfc);
        await _unitOfWork.Proveedores.AddAsync(entity);
        await _unitOfWork.CompleteAsync();
        return Map(entity);
    }

    public async Task<ProveedorDto> UpdateAsync(int id, GuardarProveedorDto dto)
    {
        var entity = await _unitOfWork.Proveedores.GetByIdAsync(id) ?? throw new KeyNotFoundException("El proveedor no existe.");
        var rfc = NormalizeRfc(dto.Rfc);
        var existing = await _unitOfWork.Proveedores.GetByRfcAsync(rfc);
        if (existing is not null && existing.Id != id) throw new InvalidOperationException("Ya existe un proveedor con ese RFC.");
        Apply(entity, dto, rfc);
        entity.Activo = true;
        await _unitOfWork.Proveedores.UpdateAsync(entity);
        await _unitOfWork.CompleteAsync();
        return Map(entity);
    }

    public async Task DeactivateAsync(int id)
    {
        var entity = await _unitOfWork.Proveedores.GetByIdAsync(id) ?? throw new KeyNotFoundException("El proveedor no existe.");
        entity.Activo = false;
        await _unitOfWork.Proveedores.UpdateAsync(entity);
        await _unitOfWork.CompleteAsync();
    }

    private static string NormalizeRfc(string rfc) => rfc.Trim().ToUpperInvariant();
    private static void Apply(Proveedor entity, GuardarProveedorDto dto, string rfc)
    {
        entity.NombreEmpresa = dto.NombreEmpresa.Trim(); entity.Contacto = dto.Contacto?.Trim();
        entity.Telefono = dto.Telefono?.Trim(); entity.Email = dto.Email?.Trim(); entity.Rfc = rfc;
    }
    private static ProveedorDto Map(Proveedor p) => new() { Id = p.Id, NombreEmpresa = p.NombreEmpresa, Contacto = p.Contacto, Telefono = p.Telefono, Email = p.Email, Rfc = p.Rfc, Activo = p.Activo };
}
