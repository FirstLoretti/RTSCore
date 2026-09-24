using MediatR;

using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Campaign.UnitRecruitment;

public record RecruitRegularUnitCommand(
    ArmyId ArmyId,
    UnitType Type,
    FactionType Faction
) : IRequest;
