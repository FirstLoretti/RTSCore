using MediatR;

using RTSCore.Domain;

namespace RTSCore.Application.Campaign.Lifecycle;

public record EndTurnCommand(FactionType FactionType) : IRequest;