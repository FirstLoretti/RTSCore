using MediatR;

using RTSCore.Application.Campaign.Common;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Campaign.UnitRecruitment;

public record GetCityRecruitOptionsQuery(CityId CityId)
    : IRequest<IReadOnlyCollection<CityCatalogOptionDto<UnitType>>>, ICityQuery;