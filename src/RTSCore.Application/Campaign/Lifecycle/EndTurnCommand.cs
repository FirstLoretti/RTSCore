using MediatR;

using RTSCore.Domain.Enums;

namespace RTSCore.Application.Campaign.Lifecycle;

public record EndTurnCommand(FactionType FactionType) : IRequest;