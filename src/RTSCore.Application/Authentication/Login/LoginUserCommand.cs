using MediatR;

using RTSCore.Domain.Common;

namespace RTSCore.Application.Authentication.Login;

public record LoginUserCommand(string Name, string Password) : IRequest<Result<AuthResponse>>;