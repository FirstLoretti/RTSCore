using MediatR;

namespace RTSCore.Application.Authentication.Commands;

public record LoginUserCommand(string Name, string Password) : IRequest<string>;