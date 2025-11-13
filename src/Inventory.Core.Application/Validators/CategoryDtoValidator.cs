using AAM.Inventory.Core.Application.DTOs;
using FluentValidation;

namespace AAM.Inventory.Core.Application.Validators;

/// <summary>
/// Validator for CategoryDto
/// </summary>
public class CategoryDtoValidator : AbstractValidator<CategoryDto>
{
    public CategoryDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required")
            .MaximumLength(100).WithMessage("Category name cannot exceed 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));
    }
}

