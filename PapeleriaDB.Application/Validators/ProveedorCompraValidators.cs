using FluentValidation;
using PapeleriaDB.Application.DTOs;

namespace PapeleriaDB.Application.Validators;

public class GuardarProveedorDtoValidator : AbstractValidator<GuardarProveedorDto>
{
    public GuardarProveedorDtoValidator()
    {
        RuleFor(x => x.NombreEmpresa).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Rfc).NotEmpty().MinimumLength(3).MaximumLength(20);
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.Telefono).MaximumLength(30);
    }
}

public class RegistrarCompraDtoValidator : AbstractValidator<RegistrarCompraDto>
{
    public RegistrarCompraDtoValidator()
    {
        RuleFor(x => x.ProveedorId).GreaterThan(0);
        RuleFor(x => x.Detalles).NotEmpty().WithMessage("Agrega al menos un producto.");
        RuleForEach(x => x.Detalles).ChildRules(d => { d.RuleFor(x => x.ProductoId).GreaterThan(0); d.RuleFor(x => x.Cantidad).GreaterThan(0); d.RuleFor(x => x.CostoUnitario).GreaterThanOrEqualTo(0); });
        RuleFor(x => x.FolioProveedor).MaximumLength(60);
        RuleFor(x => x.Notas).MaximumLength(500);
    }
}
