using MediatR;

namespace RTSCore.Application.Cities.Queries;

public record GetConstructionOptionsQuery(string CityId) : IRequest<IEnumerable<ConstructionOptionDto>>;