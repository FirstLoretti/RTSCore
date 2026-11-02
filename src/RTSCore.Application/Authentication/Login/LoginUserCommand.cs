using MediatR;

namespace RTSCore.Application.Authentication.Login;

public record LoginUserCommand(string Name, string Password) : IRequest<AuthResponse>;