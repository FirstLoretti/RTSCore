using MediatR;

using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Authentication.Commands;

public record RegisterUserCommand(string Name, string Password, FactionType Faction) : IRequest<string>;