using FluentValidation;

namespace RTSCore.Application.Authentication.Commands;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(c => c.Name).NotEmpty().MaximumLength(25);
        RuleFor(c => c.Password).NotEmpty().Length(8, 25);
    }
}