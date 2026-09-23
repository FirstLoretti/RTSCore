using MediatR;

using RTSCore.Domain.Interfaces;

namespace RTSCore.Application.Authentication.Tokens;

public class LogoutUserCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<LogoutUserCommand>
{
    public async Task Handle(LogoutUserCommand request, CancellationToken cancellationToken)
    {
        await unitOfWork.RefreshTokenRepository.RemoveTokensAsync(request.UserId, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}