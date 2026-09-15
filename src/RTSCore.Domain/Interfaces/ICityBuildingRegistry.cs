using RTSCore.Domain.ValueObjects;

namespace RTSCore.Domain.Interfaces;

public interface ICityBuildingRegistry
{
    IReadOnlyCollection<BuildingType> GetBuildingOptions(CityType type);
}