using MediatR;

using RTSCore.Domain.ValueObjects.Enums;

namespace RTSCore.Application.Campaign.Lifecycle;

public record EndTurnCommand(FactionType FactionType) : IRequest;