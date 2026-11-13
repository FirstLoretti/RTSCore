using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

using Ardalis.GuardClauses;

using MediatR;

using Microsoft.IdentityModel.JsonWebTokens;

using RTSCore.Application.Common.Exceptions;
using RTSCore.Application.Repositories;

namespace RTSCore.Application.Authentication.Refresh;

public sealed class RefreshTokensCommandHandler(
    IUnitOfWork unitOfWork,
    IRefreshTokenGenerator refreshTokenGenerator,
    IJwtTokenGenerator jwtTokenGenerator
) : IRequestHandler<RefreshTokensCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(RefreshTokensCommand request, CancellationToken ct)
    {
        var userId = ValidateAccessToken(request);

        var refreshToken = await unitOfWork.RefreshTokenRepository.GetAsync(request.RefreshToken, ct);
        await ValidateRefreshToken(refreshToken, userId, ct);
        refreshToken.Use();

        var user = await unitOfWork.UserRepository.GetAsync(userId, ct);
        Guard.Against.NotFound(userId, user);

        string newAccessToken = jwtTokenGenerator.Generate(user);
        string newRefreshToken = refreshTokenGenerator.Generate();

        var newRefreshTokenEntity = new RefreshToken(userId, newRefreshToken, DateTime.UtcNow.AddDays(30));

        unitOfWork.RefreshTokenRepository.Add(newRefreshTokenEntity);
        await unitOfWork.SaveChangesAsync(ct);

        return new AuthResponse(newAccessToken, newRefreshToken);
    }

    private UserId ValidateAccessToken(RefreshTokensCommand request)
    {
        ClaimsPrincipal principal;
        try
        {
            principal = jwtTokenGenerator.GetClaimsFromExpiredToken(request.AccessToken);
        }
        catch
        {
            throw new UnauthorizedException("Invalid access token.");
        }

        var userIdClaim = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        return !Guid.TryParse(userIdClaim, out var userId)
            ? throw new UnauthorizedException("Invalid token payload.")
            : new UserId(userId);
    }

    private async Task ValidateRefreshToken([NotNull] RefreshToken? refreshToken, UserId userId, CancellationToken ct)
    {
        if (refreshToken == null || refreshToken.UserId != userId)
        {
            throw new UnauthorizedException("Invalid data. Security alert.");
        }
        if (refreshToken.IsUsed || refreshToken.IsRevoked)
        {
            await RevokeTokensAndSave(userId, ct);

            throw new UnauthorizedException("Security breach detected. All sessions revoked. Please log in again.");
        }
        if (refreshToken.IsExpired)
        {
            await RevokeTokensAndSave(userId, ct);

            throw new UnauthorizedException("Session expired. Please log in again.");
        }
    }

    private async Task RevokeTokensAndSave(UserId userId, CancellationToken ct)
    {
        await unitOfWork.RefreshTokenRepository.RemoveTokensAsync(userId, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }
}