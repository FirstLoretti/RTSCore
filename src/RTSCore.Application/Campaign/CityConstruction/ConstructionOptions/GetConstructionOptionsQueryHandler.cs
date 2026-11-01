using System.Collections.Immutable;

using Ardalis.GuardClauses;

using MediatR;

using RTSCore.Application.Repositories;
using RTSCore.Domain.Configurations.Cities;
using RTSCore.Domain.Entities.Cities;

namespace RTSCore.Application.Campaign.CityConstruction.ConstructionOptions;

public sealed class GetConstructionOptionsQueryHandler(
    ICityRepository repository,
    BuildingCatalog catalog
) : IRequestHandler<GetConstructionOptionsQuery, ImmutableArray<ProductionOption<BuildingType>>>
{
    public async Task<ImmutableArray<ProductionOption<BuildingType>>> Handle(
        GetConstructionOptionsQuery request,
        CancellationToken ct
    )
    {
        var city = await repository.GetAsync(request.CityId, ct, trackChanges: false);
        Guard.Against.NotFound(request.CityId, city);

        return city.GetConstructableBuildings(catalog.Templates);
    }
}