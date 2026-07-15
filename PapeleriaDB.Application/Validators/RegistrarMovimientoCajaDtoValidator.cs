using FluentValidation;
using PapeleriaDB.Application.DTOs;

namespace PapeleriaDB.Application.Validators;

public class RegistrarMovimientoCajaDtoValidator : AbstractValidator<RegistrarMovimientoCajaDto>
{
    public RegistrarMovimientoCajaDtoValidator()
    {
        RuleFor(x => x.CajaId).GreaterThan(0);
        RuleFor(x => x.Tipo).Must(t => t is "Ingreso" or "Egreso").WithMessage("El tipo debe ser Ingreso o Egreso.");
        RuleFor(x => x.Monto).GreaterThan(0).WithMessage("El monto debe ser mayor que cero.");
        RuleFor(x => x.Motivo).NotEmpty().MaximumLength(250);
    }
}
