using MediatR;

using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Buildings.Commands;

public record struct ConstructBuildingCommand(
    string CityId,
    BuildingType BuildingType
) : IRequest;