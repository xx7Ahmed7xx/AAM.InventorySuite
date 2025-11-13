using AAM.Inventory.Core.Application.DTOs;
using FluentValidation;

namespace AAM.Inventory.Core.Application.Validators;

/// <summary>
/// Validator for UpdateProductDto
/// </summary>
public class UpdateProductDtoValidator : AbstractValidator<UpdateProductDto>
{
    public UpdateProductDtoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Product ID must be greater than zero");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required")
            .MaximumLength(200).WithMessage("Product name cannot exceed 200 characters");

        RuleFor(x => x.SKU)
            .NotEmpty().WithMessage("SKU is required")
            .MaximumLength(100).WithMessage("SKU cannot exceed 100 characters")
            .Matches(@"^[A-Za-z0-9\-_]+$").WithMessage("SKU can only contain letters, numbers, hyphens, and underscores");

        RuleFor(x => x.Barcode)
            .MaximumLength(100).WithMessage("Barcode cannot exceed 100 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Barcode));

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero");

        RuleFor(x => x.Cost)
            .GreaterThanOrEqualTo(0).WithMessage("Cost cannot be negative");

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0).WithMessage("Quantity cannot be negative")
            .When(x => x.Quantity.HasValue);

        RuleFor(x => x.MinimumStockLevel)
            .GreaterThanOrEqualTo(0).WithMessage("Minimum stock level cannot be negative");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));
    }
}

