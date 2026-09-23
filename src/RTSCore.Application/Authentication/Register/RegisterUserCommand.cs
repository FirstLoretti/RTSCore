using MediatR;

using RTSCore.Application.Authentication.Common;

namespace RTSCore.Application.Authentication.Register;

public record RegisterUserCommand(string Name, string Password) : IRequest<AuthResponse>;