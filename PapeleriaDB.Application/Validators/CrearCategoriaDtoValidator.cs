using FluentValidation;
using PapeleriaDB.Application.DTOs;

namespace PapeleriaDB.Application.Validators;

public class CrearCategoriaDtoValidator : AbstractValidator<CrearCategoriaDto>
{
    public CrearCategoriaDtoValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre de la categoría es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");

        RuleFor(x => x.Descripcion)
            .MaximumLength(250).WithMessage("La descripción no puede exceder los 250 caracteres.");
    }
}
