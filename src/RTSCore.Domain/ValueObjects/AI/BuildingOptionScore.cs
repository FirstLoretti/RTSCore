namespace RTSCore.Domain.ValueObjects.AI;

public record BuildingOptionScore(CityId CityId, BuildingType BuildingType, int Cost, int Score);