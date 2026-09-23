using FluentValidation;

namespace RTSCore.Application.Authentication.Tokens;

public class LogoutUserValidator : AbstractValidator<LogoutUserCommand>
{
    public LogoutUserValidator()
    {
        RuleFor(c => c.UserId).NotEmpty();
    }
}