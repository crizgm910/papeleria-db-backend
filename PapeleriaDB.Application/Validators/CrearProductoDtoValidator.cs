using FluentValidation;
using PapeleriaDB.Application.DTOs;

namespace PapeleriaDB.Application.Validators;

public class CrearProductoDtoValidator : AbstractValidator<CrearProductoDto>
{
    public CrearProductoDtoValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre del producto es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");

        RuleFor(x => x.CodigoInterno)
            .MaximumLength(100).WithMessage("El código interno no puede exceder los 100 caracteres.");

        RuleFor(x => x.CodigoBarras)
            .MaximumLength(100).WithMessage("El código de barras no puede exceder los 100 caracteres.");

        RuleFor(x => x.PrecioVenta)
            .GreaterThan(0).WithMessage("El precio de venta debe ser mayor a 0.");

        RuleFor(x => x.CostoCompra)
            .GreaterThanOrEqualTo(0).WithMessage("El costo de compra no puede ser negativo.");

        RuleFor(x => x.StockActual)
            .GreaterThanOrEqualTo(0).WithMessage("El stock actual no puede ser negativo.");

        RuleFor(x => x.StockMinimo)
            .GreaterThanOrEqualTo(0).WithMessage("El stock mínimo no puede ser negativo.");
    }
}
