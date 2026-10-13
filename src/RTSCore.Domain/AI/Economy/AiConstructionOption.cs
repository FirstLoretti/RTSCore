using RTSCore.Domain.Enums;
using RTSCore.Domain.ValueObjects.Identifiers;

namespace RTSCore.Domain.AI.Economy;

public record AiConstructionOption(CityId CityId, BuildingType BuildingType, int Cost, int Utility);