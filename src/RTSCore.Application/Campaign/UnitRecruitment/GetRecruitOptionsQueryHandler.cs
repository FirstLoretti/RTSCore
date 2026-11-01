using System.Collections.Immutable;

using Ardalis.GuardClauses;

using MediatR;

using RTSCore.Application.Repositories;
using RTSCore.Domain.Configurations.Armies;
using RTSCore.Domain.Entities.Armies;
using RTSCore.Domain.Entities.Cities;

namespace RTSCore.Application.Campaign.UnitRecruitment;

public sealed class GetRecruitOptionsQueryHandler(
    ICityRepository repository,
    UnitCatalog catalog
) : IRequestHandler<GetRecruitOptionsQuery, ImmutableArray<ProductionOption<UnitType>>>
{
    public async Task<ImmutableArray<ProductionOption<UnitType>>> Handle(
        GetRecruitOptionsQuery request,
        CancellationToken ct
    )
    {
        var city = await repository.GetAsync(request.CityId, ct, false);
        Guard.Against.NotFound(request.CityId, city);

        return city.GetRecruitableUnits(catalog.Templates);
    }
}