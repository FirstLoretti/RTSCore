using FluentValidation;

namespace RTSCore.Application.Authentication.Tokens;

public class RefreshTokensCommandValidator : AbstractValidator<RefreshTokensCommand>
{
    public RefreshTokensCommandValidator()
    {
        RuleFor(c => c.AccessToken).NotEmpty().MaximumLength(500);
        RuleFor(c => c.RefreshToken).NotEmpty().MaximumLength(500);
    }
}