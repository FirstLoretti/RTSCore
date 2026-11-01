using MediatR;

using RTSCore.Domain;

namespace RTSCore.Application.Campaign.Lifecycle;

public sealed record EndTurnCommand(FactionType Faction) : IRequest;