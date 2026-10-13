using MediatR;

using RTSCore.Domain.Enums;
using RTSCore.Domain.ValueObjects.Identifiers;

namespace RTSCore.Application.Campaign.CityConstruction.StartConstruction;

public record struct ConstructBuildingCommand(
    CityId CityId,
    BuildingType BuildingType
) : IRequest;