using RTSCore.Domain.Interfaces;
using RTSCore.Domain.Services;

namespace RTSCore.Domain.ValueObjects;

public class CityBuildingRegistry : ICityBuildingRegistry
{
    public IReadOnlyCollection<BuildingType> GetBuildingOptions(CityType type)
    {
        return [.. GameBalance.Cities.GetBuildingOptions(type)];
    }
}