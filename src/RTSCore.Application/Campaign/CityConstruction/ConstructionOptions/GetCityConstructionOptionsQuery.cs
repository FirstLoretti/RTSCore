using MediatR;

using RTSCore.Domain.Entities.Cities;

namespace RTSCore.Application.Campaign.CityConstruction.ConstructionOptions;

public record GetCityConstructionOptionsQuery(
    CityId CityId
) : IRequest<IReadOnlyCollection<ProductionOption>>;