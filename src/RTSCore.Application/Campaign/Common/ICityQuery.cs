using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Campaign.Common;

public interface ICityQuery
{
    CityId CityId { get; }
}