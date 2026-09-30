using MediatR;

using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Campaign.UnitRecruitment;

public record GetCityRecruitOptionsQuery(
    CityId CityId
) : IRequest<IReadOnlyCollection<ProductionOption>>;