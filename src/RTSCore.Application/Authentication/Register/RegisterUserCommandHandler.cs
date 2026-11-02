using MediatR;

using RTSCore.Application.Common.Exceptions;
using RTSCore.Application.Repositories;

namespace RTSCore.Application.Authentication.Register;

public sealed class RegisterUserCommandHandler(
    IUnitOfWork unitOfWork,
    IJwtTokenGenerator jwtTokenGenerator,
    IRefreshTokenGenerator refreshTokenGenerator
) : IRequestHandler<RegisterUserCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(RegisterUserCommand request, CancellationToken ct)
    {
        var user = await CreateUser(request, ct);
        var accessToken = jwtTokenGenerator.Generate(user);
        var refreshToken = refreshTokenGenerator.Generate();

        var refreshTokenEntity = new RefreshToken(user.Id, refreshToken, DateTime.UtcNow.AddDays(30));

        unitOfWork.RefreshTokenRepository.Add(refreshTokenEntity);
        unitOfWork.UserRepository.Add(user);
        await unitOfWork.SaveChangesAsync(ct);

        return new AuthResponse(accessToken, refreshToken);
    }

    private async Task<User> CreateUser(RegisterUserCommand request, CancellationToken ct)
    {
        var isUserExist = await unitOfWork.UserRepository.ExistAsync(request.Name, ct);

        if (isUserExist) throw new ConflictException("Username already exists.");

        string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        return new User(request.Name, passwordHash);
    }
}