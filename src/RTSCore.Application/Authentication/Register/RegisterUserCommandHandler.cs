using MediatR;

using RTSCore.Application.Repositories;
using RTSCore.Domain.Common;

namespace RTSCore.Application.Authentication.Register;

public sealed class RegisterUserCommandHandler(
    IUnitOfWork unitOfWork,
    IJwtTokenProvider jwtTokenGenerator,
    IRefreshTokenGenerator refreshTokenGenerator
) : IRequestHandler<RegisterUserCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(RegisterUserCommand request, CancellationToken ct)
    {
        var user = await CreateUser(request, ct);
        if (user.IsFailure) return user.Error;

        var accessToken = jwtTokenGenerator.Generate(user.Value);
        var refreshToken = refreshTokenGenerator.Generate();
        var refreshTokenEntity = new RefreshToken(user.Value.Id, refreshToken, DateTime.UtcNow.AddDays(30));

        unitOfWork.UserRepository.Add(user.Value);
        unitOfWork.RefreshTokenRepository.Add(refreshTokenEntity);
        await unitOfWork.SaveChangesAsync(ct);

        return new AuthResponse(accessToken, refreshToken);
    }

    private async Task<Result<User>> CreateUser(RegisterUserCommand request, CancellationToken ct)
    {
        var isUserExist = await unitOfWork.UserRepository.ExistAsync(request.Name, ct);

        if (isUserExist) return AuthErrors.UserAlreadyExist(request.Name);

        return new User(request.Name, request.Password);
    }
}