using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Cities.Queries.Common;

public interface ICityQuery
{
    CityId CityId { get; }
}