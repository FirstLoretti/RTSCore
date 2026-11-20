using MediatR;

using RTSCore.Domain.Common;

namespace RTSCore.Application.Authentication.Login;

public sealed record LoginUserCommand(string Name, string Password) : IRequest<Result<AuthResponse>>
{
    public override string ToString() => $"LoginUserCommand {{ Name = {Name}, Password = [REDACTED] }}";
}