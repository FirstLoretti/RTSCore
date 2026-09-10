using System.Security.Claims;

using MediatR;

using Microsoft.IdentityModel.JsonWebTokens;

using RTSCore.Application.Authentication.Common;
using RTSCore.Application.Common;
using RTSCore.Domain.Common;
using RTSCore.Domain.Entities;
using RTSCore.Domain.Exeptions;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.Interfaces.Authentication;

namespace RTSCore.Application.Authentication.Commands;

public class RefreshTokensCommandHanlder(
    IUnitOfWork unitOfWork,
    IRefreshTokenGenerator refreshTokenGenerator,
    IJwtTokenGenerator jwtTokenGenerator
) : IRequestHandler<RefreshTokensCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(RefreshTokensCommand request, CancellationToken cancellationToken)
    {
        ClaimsPrincipal principal;
        try
        {
            principal = jwtTokenGenerator.GetPrincipalFromExpiredToken(request.AccessToken);
        }
        catch
        {
            throw new GameRuleException("Invalid access token.");
        }

        var userIdClaim = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            throw new GameRuleException("Invalid access token.");
        }

        var refreshTokenEntity =
            await unitOfWork.RefreshTokenRepository.GetTokenAsync(request.RefreshToken, cancellationToken);

        if (refreshTokenEntity == null || DateTime.UtcNow > refreshTokenEntity.ExpiryTime || refreshTokenEntity.UserId != userId)
        {
            throw new GameRuleException("Session expired. Please log in again.");
        }
        if (refreshTokenEntity.IsUsed)
        {
            await unitOfWork.RefreshTokenRepository.RemoveTokensAsync(userId, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            throw new GameRuleException("Security breach detected. Please log in again.");
        }

        refreshTokenEntity.Use();

        var user = await unitOfWork.UserRepository.GetByIdAsync(userId, cancellationToken);
        Guard.Against.NotFound(user, userId);

        string newAccessToken = jwtTokenGenerator.Generate(user);
        string newRefreshToken = refreshTokenGenerator.Generate();

        var newRefreshTokenEntity = new RefreshToken(userId, newRefreshToken, DateTime.UtcNow.AddDays(30));

        unitOfWork.RefreshTokenRepository.Add(newRefreshTokenEntity);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthResponse(newAccessToken, newRefreshToken);
    }
}