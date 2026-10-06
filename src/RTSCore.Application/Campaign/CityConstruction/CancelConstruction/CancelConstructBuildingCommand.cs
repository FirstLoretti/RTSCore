using MediatR;

using RTSCore.Domain.ValueObjects.Identifiers;

namespace RTSCore.Application.Campaign.CityConstruction.CancelConstruction;

public record CancelConstructBuildingCommand(
    BuildingId BuildingId,
    CityId CityId
) : IRequest;