using MediatR;

using RTSCore.Application.Common.Configurations;
using RTSCore.Application.Common.Settings;
using RTSCore.Domain.Common;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Campaign.CityConstruction.ConstructionOptions;

public class GetCityConstructionOptionsQueryHandler(
    ICityRepository repository,
    BuildingConfiguration configuration
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