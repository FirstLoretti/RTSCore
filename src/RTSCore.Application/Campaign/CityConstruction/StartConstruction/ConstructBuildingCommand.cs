using MediatR;

using RTSCore.Domain.Common;
using RTSCore.Domain.Entities.Cities;

namespace RTSCore.Application.Campaign.CityConstruction.StartConstruction;

public sealed record ConstructBuildingCommand(
    CityId CityId,
    BuildingType BuildingType
) : IRequest<Result<BuildingId>>;