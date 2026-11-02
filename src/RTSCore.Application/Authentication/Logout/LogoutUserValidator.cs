using FluentValidation;

namespace RTSCore.Application.Authentication.Logout;

public sealed class LogoutUserValidator : AbstractValidator<LogoutUserCommand>
{
    public LogoutUserValidator()
    {
        RuleFor(c => c.UserId).NotEmpty();
    }
}