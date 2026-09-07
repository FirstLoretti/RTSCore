using MediatR;

using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Campaign.Commands;

public record EndTurnCommand(FactionType Faction) : IRequest;