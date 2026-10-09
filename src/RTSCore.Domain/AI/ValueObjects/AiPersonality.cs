namespace RTSCore.Domain.AI.ValueObjects;

public record AiPersonality(
    AiStrategicType StrategicType,
    BudgetWeights BudgetWeights,
    UnitWeights UnitWeights,
    BuildingWeights BuildingWeights,
    DiplomacyWeights DiplomacyWeights
);