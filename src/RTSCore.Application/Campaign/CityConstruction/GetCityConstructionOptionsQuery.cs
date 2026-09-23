using MediatR;

using RTSCore.Application.Campaign.Common;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Campaign.CityConstruction;

public record GetCityConstructionOptionsQuery(CityId CityId)
    : IRequest<IReadOnlyCollection<CityCatalogOptionDto<BuildingType>>>, ICityQuery;