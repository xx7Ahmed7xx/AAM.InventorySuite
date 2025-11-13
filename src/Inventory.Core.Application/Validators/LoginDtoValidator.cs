using AAM.Inventory.Core.Application.DTOs;
using FluentValidation;

namespace AAM.Inventory.Core.Application.Validators;

/// <summary>
/// Validator for LoginDto
/// </summary>
public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required");
    }
}

