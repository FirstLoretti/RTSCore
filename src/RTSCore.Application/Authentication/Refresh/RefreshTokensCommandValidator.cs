using FluentValidation;

namespace RTSCore.Application.Authentication.Refresh;

public sealed class RefreshTokensCommandValidator : AbstractValidator<RefreshTokensCommand>
{
    public RefreshTokensCommandValidator()
    {
        RuleFor(c => c.AccessToken).NotEmpty().MaximumLength(2000);
        RuleFor(c => c.RefreshToken).NotEmpty().MaximumLength(100);
    }
}