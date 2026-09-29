using RTSCore.Domain.Interfaces;
using RTSCore.Domain.ValueObjects.Configurations;

namespace RTSCore.Domain.ValueObjects;

public class CityBuildingRegistry(BuildingConfiguration configuration) : ICityBuildingRegistry
{
    public IReadOnlyCollection<BuildingType> GetBuildingOptions(CityType type)
    {
        return [.. configuration.Buildings
            .Where(b => b.AllowedCityTypes.Contains(type))
            .Select(b => b.Type)];
    }
}