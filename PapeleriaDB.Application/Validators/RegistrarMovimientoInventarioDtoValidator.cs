using FluentValidation;
using PapeleriaDB.Application.DTOs;

namespace PapeleriaDB.Application.Validators;

public class RegistrarMovimientoInventarioDtoValidator : AbstractValidator<RegistrarMovimientoInventarioDto>
{
    public RegistrarMovimientoInventarioDtoValidator()
    {
        RuleFor(x => x.ProductoId).GreaterThan(0);
        RuleFor(x => x.Tipo).Must(x => x is "Entrada" or "Ajuste").WithMessage("El tipo debe ser Entrada o Ajuste.");
        RuleFor(x => x.Cantidad).NotEqual(0).WithMessage("La cantidad no puede ser cero.");
        RuleFor(x => x.Motivo).NotEmpty().MaximumLength(250);
    }
}
