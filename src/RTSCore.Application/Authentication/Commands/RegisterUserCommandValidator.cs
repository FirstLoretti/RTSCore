using FluentValidation;

using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Authentication.Commands;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(c => c.Name).NotEmpty().Length(1, 25);
        RuleFor(c => c.Password).NotEmpty().Length(8, 25);
        RuleFor(c => c.Faction).IsInEnum().NotEqual(FactionType.None);
    }
}