using MediatR;

using RTSCore.Domain.Exeptions;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.Interfaces.Authentication;

namespace RTSCore.Application.Authentication.Commands;

public class LoginUserCommandHandler(IUnitOfWork unitOfWork, IJwtTokenGenerator jwtTokenGenerator)
    : IRequestHandler<LoginUserCommand, string>
{
    public async Task<string> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await unitOfWork.UserRepository.GetByNameAsync(request.Name, cancellationToken)
            ?? throw new GameRuleException("Неверное имя пользователя или пароль.");

        var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

        return isPasswordValid
            ? jwtTokenGenerator.Generate(user)
            : throw new GameRuleException("Неверное имя пользователя или пароль.");
    }
}