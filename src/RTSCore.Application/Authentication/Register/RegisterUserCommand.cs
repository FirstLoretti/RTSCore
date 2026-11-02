using MediatR;

namespace RTSCore.Application.Authentication.Register;

public sealed record RegisterUserCommand(string Name, string Password) : IRequest<AuthResponse>;