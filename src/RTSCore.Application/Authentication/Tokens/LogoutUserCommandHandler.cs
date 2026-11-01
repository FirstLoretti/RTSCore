using MediatR;

using RTSCore.Application.Repositories;

namespace RTSCore.Application.Authentication.Tokens;

public class LogoutUserCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<LogoutUserCommand>
{
    public async Task Handle(LogoutUserCommand request, CancellationToken cancellationToken)
    {
        await unitOfWork.RefreshTokenRepository.RemoveTokensAsync(request.UserId, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}