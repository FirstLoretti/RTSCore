using MediatR;

using RTSCore.Domain.Entities.Cities;

namespace RTSCore.Application.Campaign.CityConstruction.CancelConstruction;

public record CancelConstructBuildingCommand(
    BuildingId BuildingId,
    CityId CityId
) : IRequest;