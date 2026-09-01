using MediatR;

using RTSCore.Application.Cities.Queries;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Units.Queries;

public record GetCityRecruitOptionsQuery(CityId CityId)
    : IRequest<IReadOnlyCollection<CityCatalogOptionDto<UnitType>>>, ICityQuery;