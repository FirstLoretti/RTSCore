using MediatR;

using RTSCore.Domain.Entities.Armies;

namespace RTSCore.Application.Campaign.UnitRecruitment;

public sealed record CancelRecruitCommand(ArmyId ArmyId, UnitId UnitId) : IRequest;