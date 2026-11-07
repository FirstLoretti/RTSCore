using MediatR;

using RTSCore.Domain.Common;
using RTSCore.Domain.Entities.Armies;

namespace RTSCore.Application.Campaign.ArmyMovement;

public sealed record MoveArmyCommand(ArmyId Id, float X, float Y) : IRequest<Result>;