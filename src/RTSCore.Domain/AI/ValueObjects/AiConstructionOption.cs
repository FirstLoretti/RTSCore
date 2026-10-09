using RTSCore.Domain.ValueObjects.Enums;
using RTSCore.Domain.ValueObjects.Identifiers;

namespace RTSCore.Domain.AI.ValueObjects;

public record AiConstructionOption(CityId CityId, BuildingType BuildingType, int Cost, int Utility);