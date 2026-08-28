using MediatR;

using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Cities.Commands;

public record struct ConstructBuildingCommand(
    string CityId,
    BuildingType BuildingType
) : IRequest;