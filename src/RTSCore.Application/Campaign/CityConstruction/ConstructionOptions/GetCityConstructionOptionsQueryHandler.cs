using MediatR;

using RTSCore.Application.Common.Validation;
using RTSCore.Domain.Common;
using RTSCore.Domain.Entities.Campaign;
using RTSCore.Domain.ValueObjects.Common;
using RTSCore.Domain.ValueObjects.Configurations;

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