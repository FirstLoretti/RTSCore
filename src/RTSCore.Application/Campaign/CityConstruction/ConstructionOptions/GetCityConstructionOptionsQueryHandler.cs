using MediatR;

using RTSCore.Domain.Common.Exceptions;
using RTSCore.Domain.Common.Repositories;
using RTSCore.Domain.Configurations.Cities;
using RTSCore.Domain.Entities.Cities;

namespace RTSCore.Application.Campaign.CityConstruction.ConstructionOptions;

public class GetCityConstructionOptionsQueryHandler(
    ICityRepository repository,
    BuildingCatalog configuration
) : IRequestHandler<GetCityConstructionOptionsQuery, IReadOnlyCollection<ProductionOption>>
{
    public async Task<IReadOnlyCollection<ProductionOption>> Handle(
        GetCityConstructionOptionsQuery request,
        CancellationToken ct
    )
    {
        var city = await repository.GetReadOnlyAsync(request.CityId, ct);
        Guard.Against.NotFound(city, request.CityId);

        return city.GetConstructableBuildings(configuration.Templates);
    }
}