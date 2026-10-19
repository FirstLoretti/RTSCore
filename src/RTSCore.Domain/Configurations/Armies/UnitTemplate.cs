using RTSCore.Domain.Entities.Armies;
using RTSCore.Domain.Entities.Cities;

namespace RTSCore.Domain.Configurations.Armies;

public sealed record UnitTemplate
{
    public required UnitType Type { get; init; }
    public required string DisplayName { get; init; } = string.Empty;
    public required int Cost { get; init; }
    public required int MaxHealth { get; init; }
    public required int Damage { get; init; }
    public required int Armor { get; init; }
    public required int Speed { get; init; }
    public required int ExpKillReward { get; init; }
    public required float HealthGrowthRate { get; init; }
    public required float DamageGrowthRate { get; init; }
    public required int TurnsToRecruit { get; init; }
    public required int AiUtility { get; init; }
    public required UnitCategory Category { get; init; }
    public required ProductionCategory ProductionCategory { get; init; }

    public BuildingType RequiredBuilding { get; init; }
}