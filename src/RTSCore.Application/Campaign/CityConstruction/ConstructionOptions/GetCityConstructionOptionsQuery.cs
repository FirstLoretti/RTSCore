using MediatR;

using RTSCore.Domain.ValueObjects.Common;
using RTSCore.Domain.ValueObjects.Identifiers;

namespace RTSCore.Application.Campaign.CityConstruction.ConstructionOptions;

public record GetCityConstructionOptionsQuery(
    CityId CityId
) : IRequest<IReadOnlyCollection<ProductionOption>>;