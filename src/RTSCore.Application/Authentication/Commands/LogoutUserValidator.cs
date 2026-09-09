using FluentValidation;

namespace RTSCore.Application.Authentication.Commands;

public class LogoutUserValidator : AbstractValidator<LogoutUserCommand>
{
    public LogoutUserValidator()
    {
        RuleFor(c => c.UserId).NotEmpty();
    }
}