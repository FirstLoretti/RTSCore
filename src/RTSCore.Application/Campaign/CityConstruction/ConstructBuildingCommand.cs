using MediatR;

using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Campaign.CityConstruction;

public record struct ConstructBuildingCommand(
    string CityId,
    BuildingType BuildingType
) : IRequest;