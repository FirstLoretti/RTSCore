using MediatR;

using RTSCore.Domain.Common.Exceptions;
using RTSCore.Domain.Common.Repositories;
using RTSCore.Domain.Configurations;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Campaign.UnitRecruitment;

public class GetCityRecruitOptionsQueryHandler(
    ICityRepository repository,
    UnitConfig unitConfiguration
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