using MediatR;

using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Units.Commands;

public readonly record struct CreateUnitCommand(
    string Id,
    UnitType Type,
    FactionType Faction
) : IRequest<UnitId>;
