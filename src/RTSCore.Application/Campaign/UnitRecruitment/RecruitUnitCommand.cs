using MediatR;

using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Campaign.UnitRecruitment;

public record RecruitUnitCommand(string ArmyId, UnitType Type, FactionType Faction) : IRequest;
