namespace RTSCore.Domain.ValueObjects.AI;

public record AiPersonality(
    AiStrategicType StrategicType,
    BudgetWeights BudgetWeights,
    UnitWeights UnitWeights,
    BuildingWeights BuildingWeights,
    DiplomacyWeights DiplomacyWeights
);