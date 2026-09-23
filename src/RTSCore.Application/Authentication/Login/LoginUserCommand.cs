using MediatR;

using RTSCore.Application.Authentication.Common;

namespace RTSCore.Application.Authentication.Login;

public record LoginUserCommand(string Name, string Password) : IRequest<AuthResponse>;