using MediatR;

using RTSCore.Application.Authentication.Common;
using RTSCore.Domain.Entities;
using RTSCore.Domain.Exeptions;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.Interfaces.Authentication;

namespace RTSCore.Application.Authentication.Commands;

public class RegisterUserCommandHandler(
    IUnitOfWork unitOfWork,
    IJwtTokenGenerator jwtTokenGenerator,
    IRefreshTokenGenerator refreshTokenGenerator
)
: IRequestHandler<RegisterUserCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var user = await CreateUser(request, cancellationToken);
        var accessToken = jwtTokenGenerator.Generate(user);
        var refreshToken = refreshTokenGenerator.Generate();

        var refreshTokenEntity = new RefreshToken(user.Id, refreshToken, DateTime.UtcNow.AddDays(30));

        unitOfWork.RefreshTokenRepository.Add(refreshTokenEntity);
        unitOfWork.UserRepository.Add(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthResponse(accessToken, refreshToken);
    }

    private async Task<User> CreateUser(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var isUserExist = await unitOfWork.UserRepository.ExistAsync(request.Name, cancellationToken);

        if (isUserExist) throw new GameRuleException("Игрок с таким именем уже существует.");

        string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        return new User(request.Name, passwordHash);
    }
}