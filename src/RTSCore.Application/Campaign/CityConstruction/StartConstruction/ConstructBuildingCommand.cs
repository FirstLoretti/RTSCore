using MediatR;

using RTSCore.Domain.Entities.Cities;

namespace RTSCore.Application.Campaign.CityConstruction.StartConstruction;

public record struct ConstructBuildingCommand(
    CityId CityId,
    BuildingType BuildingType
) : IRequest;