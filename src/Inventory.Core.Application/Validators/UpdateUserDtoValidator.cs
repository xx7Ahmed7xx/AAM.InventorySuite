using AAM.Inventory.Core.Application.DTOs;
using FluentValidation;

namespace AAM.Inventory.Core.Application.Validators;

/// <summary>
/// Validator for UpdateUserDto
/// </summary>
public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserDtoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("User ID must be greater than zero");

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required")
            .MinimumLength(3).WithMessage("Username must be at least 3 characters")
            .MaximumLength(100).WithMessage("Username cannot exceed 100 characters")
            .Matches(@"^[A-Za-z0-9_]+$").WithMessage("Username can only contain letters, numbers, and underscores");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(200).WithMessage("Email cannot exceed 200 characters");

        RuleFor(x => x.Password)
            .MinimumLength(6).WithMessage("Password must be at least 6 characters")
            .MaximumLength(100).WithMessage("Password cannot exceed 100 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Password));
    }
}

