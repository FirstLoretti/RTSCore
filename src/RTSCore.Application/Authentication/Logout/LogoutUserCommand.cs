using MediatR;

namespace RTSCore.Application.Authentication.Logout;

public sealed record LogoutUserCommand(UserId UserId) : IRequest;