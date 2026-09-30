using MediatR;

using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Campaign.CityConstruction.StartConstruction;

public record struct ConstructBuildingCommand(
    CityId CityId,
    BuildingType BuildingType
) : IRequest;