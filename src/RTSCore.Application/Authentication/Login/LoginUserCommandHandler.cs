using System.Diagnostics.CodeAnalysis;

using MediatR;

using RTSCore.Application.Common.Exceptions;
using RTSCore.Application.Repositories;

namespace RTSCore.Application.Authentication.Login;

public class LoginUserCommandHandler(
    IUnitOfWork unitOfWork,
    IJwtTokenGenerator jwtTokenGenerator,
    IRefreshTokenGenerator refreshTokenGenerator
) : IRequestHandler<LoginUserCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(LoginUserCommand request, CancellationToken ct)
    {
        var user = await unitOfWork.UserRepository.GetAsync(request.Name, ct);
        ThrowIfInvalidUserData(user, request.Password);

        var refreshToken = refreshTokenGenerator.Generate();
        var accessToken = jwtTokenGenerator.Generate(user);

        var refreshTokenEntity = new RefreshToken(user.Id, refreshToken, DateTime.UtcNow.AddDays(30));

        await unitOfWork.RefreshTokenRepository.RemoveTokensAsync(user.Id, ct);

        unitOfWork.RefreshTokenRepository.Add(refreshTokenEntity);
        await unitOfWork.SaveChangesAsync(ct);

        return new AuthResponse(accessToken, refreshToken);
    }

    private static void ThrowIfInvalidUserData([NotNull] User? user, string password)
    {
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            throw new UnauthorizedException("Wrong username or password.");
    }
}