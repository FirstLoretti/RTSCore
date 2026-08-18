namespace RTSCore.Domain.ValueObjects;

public readonly record struct BuildingTemplate(
    BuildingType Type,
    FactionType Faction,
    string DisplayName,
    int MaxHealth,
    int MaxRecruitmentSlots
);