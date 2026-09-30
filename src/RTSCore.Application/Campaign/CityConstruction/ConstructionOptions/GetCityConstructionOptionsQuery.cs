using MediatR;

using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Campaign.CityConstruction.ConstructionOptions;

public record GetCityConstructionOptionsQuery(
    CityId CityId
) : IRequest<IReadOnlyCollection<ProductionOption>>;