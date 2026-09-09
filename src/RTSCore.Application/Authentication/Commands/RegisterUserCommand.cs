using MediatR;

using RTSCore.Application.Authentication.Common;

namespace RTSCore.Application.Authentication.Commands;

public record RegisterUserCommand(string Name, string Password) : IRequest<AuthResponse>;