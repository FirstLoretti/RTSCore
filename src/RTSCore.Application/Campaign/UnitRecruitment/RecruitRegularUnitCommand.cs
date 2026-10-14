using MediatR;

using RTSCore.Domain;
using RTSCore.Domain.Entities.Armies;

namespace RTSCore.Application.Campaign.UnitRecruitment;

public record RecruitRegularUnitCommand(
    ArmyId ArmyId,
    UnitType Type,
    FactionType Faction
) : IRequest;
