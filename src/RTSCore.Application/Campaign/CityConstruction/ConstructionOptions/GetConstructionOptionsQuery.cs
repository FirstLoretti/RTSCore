using System.Collections.Immutable;

using MediatR;

using RTSCore.Domain.Entities.Cities;

namespace RTSCore.Application.Campaign.CityConstruction.ConstructionOptions;

public sealed record GetConstructionOptionsQuery(
    CityId CityId
) : IRequest<ImmutableArray<ProductionOption<BuildingType>>>;