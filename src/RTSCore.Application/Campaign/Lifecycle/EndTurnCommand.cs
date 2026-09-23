using MediatR;

using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Campaign.Lifecycle;

public record EndTurnCommand(FactionType Faction) : IRequest;