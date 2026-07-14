using PapeleriaDB.Application.DTOs;
using PapeleriaDB.Domain.Entities;
using PapeleriaDB.Domain.Interfaces;

namespace PapeleriaDB.Application.Services;

public class CategoriaService : ICategoriaService
{
    private readonly IUnitOfWork _unitOfWork;

    public CategoriaService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<CategoriaDto>> GetAllAsync()
    {
        var categorias = await _unitOfWork.Categorias.GetAllAsync();
        return categorias
            .OrderBy(c => c.Nombre)
            .Select(Map);
    }

    public async Task<CategoriaDto> CreateAsync(CrearCategoriaDto dto)
    {
        var nombre = dto.Nombre.Trim();
        if (await _unitOfWork.Categorias.GetByNombreAsync(nombre) is not null)
            throw new InvalidOperationException("Ya existe una categoría con ese nombre.");

        var categoria = new Categoria
        {
            Nombre = nombre,
            Descripcion = string.IsNullOrWhiteSpace(dto.Descripcion) ? null : dto.Descripcion.Trim(),
            Estado = true
        };

        await _unitOfWork.Categorias.AddAsync(categoria);
        await _unitOfWork.CompleteAsync();
        return Map(categoria);
    }

    private static CategoriaDto Map(Categoria categoria) => new()
    {
        Id = categoria.Id,
        Nombre = categoria.Nombre,
        Descripcion = categoria.Descripcion,
        Estado = categoria.Estado
    };
}
