using MediatR;

using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Cities.Commands;

public record RecruitUnitCommand(string ArmyId, UnitType Type, FactionType OwnerFaction) : IRequest;
