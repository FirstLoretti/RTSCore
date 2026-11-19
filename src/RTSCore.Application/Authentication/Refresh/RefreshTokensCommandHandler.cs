using Ardalis.GuardClauses;

using MediatR;

using RTSCore.Application.Repositories;
using RTSCore.Domain.Common;

namespace RTSCore.Application.Authentication.Refresh;

public sealed class RefreshTokensCommandHandler(
    IUnitOfWork unitOfWork,
    IRefreshTokenGenerator refreshTokenGenerator,
    IJwtTokenProvider jwtTokenProvider
) : IRequestHandler<RefreshTokensCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(RefreshTokensCommand request, CancellationToken ct)
    {
        var accessResult = jwtTokenProvider.ValidateExpiredToken(request.AccessToken);
        if (accessResult.IsFailure) return accessResult.Error;

        var user = await unitOfWork.UserRepository.GetAsync(accessResult.Value, ct);
        Guard.Against.NotFound(accessResult.Value, user);

        var refreshToken = await unitOfWork.RefreshTokenRepository.GetAsync(request.RefreshToken, ct);
        if (refreshToken == null) return AuthErrors.Unauthorized("Refresh token not found. Please log in again.");

        var refreshResult = refreshToken.Use(accessResult.Value);
        if (refreshResult.IsFailure)
        {
            if (refreshResult.Error == AuthErrors.RefreshTokenCompromized)
                await unitOfWork.RefreshTokenRepository.RemoveTokensAsync(refreshToken.UserId, ct);

            return refreshResult.Error;
        }

        string newAccessToken = jwtTokenProvider.Generate(user);
        string newRefreshToken = refreshTokenGenerator.Generate();
        var newRefreshTokenEntity = new RefreshToken(accessResult.Value, newRefreshToken, DateTime.UtcNow.AddDays(30));

        unitOfWork.RefreshTokenRepository.Add(newRefreshTokenEntity);
        await unitOfWork.SaveChangesAsync(ct);

        return new AuthResponse(newAccessToken, newRefreshToken);
    }
}