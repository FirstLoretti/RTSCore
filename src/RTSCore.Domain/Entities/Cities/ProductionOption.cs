namespace RTSCore.Domain.Entities.Cities;

public readonly record struct ProductionOption<TType>(
    string DisplayName,
    TType Type,
    int Cost,
    int TurnsToConstruct,
    ProductionCategory ProductionCategory,
    int AiUtility
) where TType : Enum;