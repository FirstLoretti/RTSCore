using MediatR;

using RTSCore.Domain.ValueObjects.Enums;
using RTSCore.Domain.ValueObjects.Identifiers;

namespace RTSCore.Application.Campaign.UnitRecruitment;

public record RecruitRegularUnitCommand(
    ArmyId ArmyId,
    UnitType Type,
    FactionType Faction
) : IRequest;
