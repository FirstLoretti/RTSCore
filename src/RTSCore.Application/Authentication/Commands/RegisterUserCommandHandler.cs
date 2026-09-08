using MediatR;

using RTSCore.Domain.Entities;
using RTSCore.Domain.Exeptions;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.Interfaces.Authentication;

namespace RTSCore.Application.Authentication.Commands;

public class RegisterUserCommandHandler(
    IUnitOfWork unitOfWork,
    IJwtTokenGenerator jwtTokenGenerator
)
: IRequestHandler<RegisterUserCommand, string>
{
    public async Task<string> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var isUserExist = await unitOfWork.UserRepository.ExistAsync(request.Name, cancellationToken);

        if (isUserExist) throw new GameRuleException("Игрок с таким именем уже существует.");

        string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User(request.Name, passwordHash, request.Faction);

        unitOfWork.UserRepository.Add(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return jwtTokenGenerator.Generate(user);
    }
}