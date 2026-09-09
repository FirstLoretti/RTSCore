using MediatR;

using RTSCore.Application.Authentication.Common;
using RTSCore.Domain.Entities;
using RTSCore.Domain.Exeptions;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.Interfaces.Authentication;

namespace RTSCore.Application.Authentication.Commands;

public class LoginUserCommandHandler(
    IUnitOfWork unitOfWork,
    IJwtTokenGenerator jwtTokenGenerator,
    IRefreshTokenGenerator refreshTokenGenerator
)
: IRequestHandler<LoginUserCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await GetUser(request, cancellationToken);

        var refreshToken = refreshTokenGenerator.Generate();
        var accessToken = jwtTokenGenerator.Generate(user);

        var refreshTokenEntity = new RefreshToken(user.Id, refreshToken, DateTime.UtcNow.AddDays(30));

        await unitOfWork.RefreshTokenRepository.RemoveTokensAsync(user.Id, cancellationToken);

        unitOfWork.RefreshTokenRepository.Add(refreshTokenEntity);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthResponse(accessToken, refreshToken);
    }

    private async Task<User> GetUser(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await unitOfWork.UserRepository.GetByNameAsync(request.Name, cancellationToken);

        return user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash)
            ? throw new GameRuleException("Неверное имя пользователя или пароль.")
            : user;
    }
}