using MediatR;

namespace RTSCore.Application.Authentication.Commands;

public record LogoutUserCommand(Guid UserId) : IRequest;