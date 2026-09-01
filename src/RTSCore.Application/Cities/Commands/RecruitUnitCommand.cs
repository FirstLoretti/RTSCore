using MediatR;

using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Cities.Commands;

public record struct RecruitUnitCommand(
    string CityId,
    UnitType Type,
    FactionType OwnerFaction
) : IRequest;
