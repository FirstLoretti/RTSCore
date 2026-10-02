using MediatR;

using RTSCore.Application.Common.Settings;
using RTSCore.Domain.Common;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.ValueObjects;
using RTSCore.Domain.ValueObjects.Configurations;

namespace RTSCore.Application.Campaign.UnitRecruitment;

public class GetCityRecruitOptionsQueryHandler(
    ICityRepository repository,
    UnitConfiguration unitConfiguration
) : IRequestHandler<GetCityRecruitOptionsQuery, IReadOnlyCollection<ProductionOption>>
{
    public async Task<IReadOnlyCollection<ProductionOption>> Handle(
        GetCityRecruitOptionsQuery request,
        CancellationToken ct
    )
    {
        var city = await repository.GetReadOnlyAsync(request.CityId, ct);
        Guard.Against.NotFound(city, request.CityId);

        return city.GetRecruitableUnits(unitConfiguration.Templates);
    }
}