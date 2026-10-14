using RTSCore.Domain.Entities.Cities;

namespace RTSCore.Domain.AI.Economy;

public readonly record struct AiConstructionOption(
    CityId CityId,
    BuildingType BuildingType,
    int Cost,
    int Utility
);