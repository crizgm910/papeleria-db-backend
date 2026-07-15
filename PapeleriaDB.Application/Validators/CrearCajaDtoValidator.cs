using FluentValidation;
using PapeleriaDB.Application.DTOs;

namespace PapeleriaDB.Application.Validators;

public class CrearCajaDtoValidator : AbstractValidator<CrearCajaDto>
{
    public CrearCajaDtoValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre de la caja es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre de la caja no puede exceder los 100 caracteres.");
    }
}
