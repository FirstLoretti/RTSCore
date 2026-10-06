using RTSCore.Domain.ValueObjects.Enums;
using RTSCore.Domain.ValueObjects.Identifiers;

namespace RTSCore.Domain.ValueObjects.AI;

public record BuildingOptionScore(CityId CityId, BuildingType BuildingType, int Cost, int Score);