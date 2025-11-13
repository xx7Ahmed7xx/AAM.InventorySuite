using AAM.Inventory.Core.Application.DTOs;
using FluentValidation;

namespace AAM.Inventory.Core.Application.Validators;

/// <summary>
/// Validator for StockMovementRequestDto
/// </summary>
public class StockMovementRequestDtoValidator : AbstractValidator<StockMovementRequestDto>
{
    public StockMovementRequestDtoValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0).WithMessage("Product ID must be greater than zero");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero");

        RuleFor(x => x.Reason)
            .MaximumLength(500).WithMessage("Reason cannot exceed 500 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Reason));

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Notes cannot exceed 1000 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Notes));

        RuleFor(x => x.CreatedBy)
            .MaximumLength(100).WithMessage("CreatedBy cannot exceed 100 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.CreatedBy));
    }
}

