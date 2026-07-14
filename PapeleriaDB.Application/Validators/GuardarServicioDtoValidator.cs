using FluentValidation;
using PapeleriaDB.Application.DTOs;

namespace PapeleriaDB.Application.Validators;

public class GuardarServicioDtoValidator : AbstractValidator<GuardarServicioDto>
{
    public GuardarServicioDtoValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
        RuleFor(x => x.CodigoInterno).MaximumLength(100);
        RuleFor(x => x.PrecioBase).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Descripcion).MaximumLength(500);
    }
}
