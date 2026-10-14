using RTSCore.Domain.Entities.Cities;

namespace RTSCore.Domain.AI.Economy;

public record AiConstructionOption(CityId CityId, BuildingType BuildingType, int Cost, int Utility);