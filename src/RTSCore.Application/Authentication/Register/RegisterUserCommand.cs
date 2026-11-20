using MediatR;

using RTSCore.Domain.Common;

namespace RTSCore.Application.Authentication.Register;

public sealed record RegisterUserCommand(string Name, string Password) : IRequest<Result<AuthResponse>>
{
    public override string ToString() => $"RegisterUserCommand {{ Name = {Name}, Password = [REDACTED] }}";
}