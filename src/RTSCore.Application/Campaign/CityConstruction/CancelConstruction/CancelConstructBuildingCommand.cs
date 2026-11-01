using MediatR;

using RTSCore.Domain.Entities.Cities;

namespace RTSCore.Application.Campaign.CityConstruction.CancelConstruction;

public sealed record CancelConstructBuildingCommand(
    BuildingId BuildingId,
    CityId CityId
) : IRequest;