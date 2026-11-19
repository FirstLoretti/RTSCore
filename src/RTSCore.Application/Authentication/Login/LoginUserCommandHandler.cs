using MediatR;

using RTSCore.Application.Repositories;
using RTSCore.Domain.Common;

namespace RTSCore.Application.Authentication.Login;

public sealed class LoginUserCommandHandler(
    IUnitOfWork unitOfWork,
    IJwtTokenProvider jwtTokenGenerator,
    IRefreshTokenGenerator refreshTokenGenerator
) : IRequestHandler<LoginUserCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(LoginUserCommand request, CancellationToken ct)
    {
        var user = await unitOfWork.UserRepository.GetAsync(request.Name, ct);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return AuthErrors.InvalidCredentials;

        var refreshToken = refreshTokenGenerator.Generate();
        var accessToken = jwtTokenGenerator.Generate(user);

        var refreshTokenEntity = new RefreshToken(user.Id, refreshToken, DateTime.UtcNow.AddDays(30));

        await unitOfWork.RefreshTokenRepository.RemoveTokensAsync(user.Id, ct);

        unitOfWork.RefreshTokenRepository.Add(refreshTokenEntity);
        await unitOfWork.SaveChangesAsync(ct);

        return new AuthResponse(accessToken, refreshToken);
    }
}