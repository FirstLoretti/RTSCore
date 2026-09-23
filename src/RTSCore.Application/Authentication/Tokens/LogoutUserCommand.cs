using MediatR;

namespace RTSCore.Application.Authentication.Tokens;

public record LogoutUserCommand(Guid UserId) : IRequest;