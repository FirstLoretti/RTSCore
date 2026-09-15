namespace RTSCore.Domain.ValueObjects.AI;

public record AiPersonality(
    AiStrategicType StrategicType,
    BuildingWeights BuildingWeights,
    DiplomacyWeights DiplomacyWeights
);