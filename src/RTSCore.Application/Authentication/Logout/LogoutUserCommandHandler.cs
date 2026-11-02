using MediatR;

using RTSCore.Application.Repositories;

namespace RTSCore.Application.Authentication.Logout;

public sealed class LogoutUserCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<LogoutUserCommand>
{
    public async Task Handle(LogoutUserCommand request, CancellationToken ct)
    {
        await unitOfWork.RefreshTokenRepository.RemoveTokensAsync(request.UserId, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }
}