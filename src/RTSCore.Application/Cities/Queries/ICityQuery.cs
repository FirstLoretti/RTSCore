using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Cities.Queries;

public interface ICityQuery
{
    CityId CityId { get; }
}