using MediatR;

using RTSCore.Application.Cities.Queries.Common;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Cities.Queries;

public record GetCityConstructionOptionsQuery(CityId CityId)
    : IRequest<IReadOnlyCollection<CityCatalogOptionDto<BuildingType>>>, ICityQuery;