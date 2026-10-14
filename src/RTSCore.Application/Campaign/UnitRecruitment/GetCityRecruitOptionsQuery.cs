using MediatR;

using RTSCore.Domain.Entities.Cities;

namespace RTSCore.Application.Campaign.UnitRecruitment;

public record GetCityRecruitOptionsQuery(
    CityId CityId
) : IRequest<IReadOnlyCollection<ProductionOption>>;