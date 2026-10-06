using RTSCore.Domain.ValueObjects.Enums;

namespace RTSCore.Domain.Entities.Campaign;

public interface ICityBuildingRegistry
{
    IReadOnlyCollection<BuildingType> GetBuildingOptions(CityType type);
}