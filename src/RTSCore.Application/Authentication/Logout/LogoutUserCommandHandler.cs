using MediatR;

using RTSCore.Application.Repositories;

namespace RTSCore.Application.Authentication.Logout;

public sealed class LogoutUserCommandHandler(IRefreshTokenRepository repository) : IRequestHandler<LogoutUserCommand>
{
    public async Task Handle(LogoutUserCommand request, CancellationToken ct) =>
        await repository.RemoveTokensAsync(request.UserId, ct);
}