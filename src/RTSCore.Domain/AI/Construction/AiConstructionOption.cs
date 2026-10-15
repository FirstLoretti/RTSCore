using RTSCore.Domain.Entities.Cities;

namespace RTSCore.Domain.AI.Construction;

public readonly record struct AiConstructionOption(
    CityId CityId,
    BuildingType BuildingType,
    int Cost,
    int Utility
);