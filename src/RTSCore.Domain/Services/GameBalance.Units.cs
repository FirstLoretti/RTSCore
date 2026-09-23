using System.Collections.Frozen;
using System.Collections.Immutable;

using RTSCore.Domain.Exeptions;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Domain.Services;

public partial class GameBalance
{
    public static class Units
    {
        public const float HealthWeight = 1.0f;
        public const float DamageWeight = 2.0f;
        public const float ArmorWeight = 2.0f;

        public static readonly ImmutableArray<int> ExpToNextLevel = [100, 200, 400];

        public static IReadOnlyCollection<UnitTemplate> GetAllTemplates => TypeToTemplate.Values;
        private static readonly FrozenDictionary<UnitType, UnitTemplate> TypeToTemplate;

        static Units()
        {
            Dictionary<UnitType, UnitTemplate> temporary = new()
            {
                {
                    UnitType.Peasant,
                    new UnitTemplate(
                        Type: UnitType.Peasant,
                        DisplayName: "Peasant",
                        Cost: 150,
                        MaxHealth: 100,
                        Damage: 25,
                        Armor: 2,
                        Speed: 5,
                        ExpKillReward: 50,
                        HealthGrowthRate: 1.1f,
                        DamageGrowthRate: 1.15f,
                        TurnsToRecruit: 1,
                        Category: UnitCategory.Infantry,
                        AiUtility: 50,
                        RequiredBuilding: BuildingType.ReqruitBarrack
                    )
                },
                {
                    UnitType.PeasantArcher,
                    new UnitTemplate(
                        Type: UnitType.PeasantArcher,
                        DisplayName: "Peasant Archer",
                        Cost: 150,
                        MaxHealth: 100,
                        Damage: 25,
                        Armor: 2,
                        Speed: 5,
                        ExpKillReward: 50,
                        HealthGrowthRate: 1.1f,
                        DamageGrowthRate: 1.15f,
                        TurnsToRecruit: 1,
                        Category: UnitCategory.RangeInfantry,
                        AiUtility: 50,
                        RequiredBuilding: BuildingType.ReqruitBarrack
                    )
                },
                {
                    UnitType.Militia,
                    new UnitTemplate(
                        Type: UnitType.Militia,
                        DisplayName: "Militia",
                        Cost: 250,
                        MaxHealth: 115,
                        Damage: 20,
                        Armor: 3,
                        Speed: 5,
                        ExpKillReward: 50,
                        HealthGrowthRate: 1.15f,
                        DamageGrowthRate: 1.10f,
                        TurnsToRecruit: 2,
                        Category: UnitCategory.Infantry,
                        AiUtility: 67,
                        RequiredBuilding: BuildingType.MilitiaBarrack
                    )
                },

                {
                    UnitType.Invulnerable,
                    new UnitTemplate(
                        Type: UnitType.Invulnerable,
                        DisplayName: "Invulnerable",
                        Cost: 0,
                        MaxHealth: 1,
                        Damage: 1,
                        Armor: 1,
                        Speed: 1,
                        ExpKillReward: 1,
                        HealthGrowthRate: 1f,
                        DamageGrowthRate: 1f,
                        TurnsToRecruit: 1,
                        UnitCategory.Infantry,
                        AiUtility: 1
                    )
                },

                {
                    UnitType.Knight,
                    new UnitTemplate(UnitType.Knight,"Knight", 1, 1, 1, 1, 1, 1, 1f, 1f, 1,UnitCategory.Infantry, 1)
                }
            };

            TypeToTemplate = temporary.ToFrozenDictionary();
        }

        public static UnitTemplate GetTemplate(UnitType type)
        {
            return !TypeToTemplate.TryGetValue(type, out var template)
                ? throw new NotFoundException(
                    $"[{nameof(Units)}] Шаблон юнита {type} не найден в системе"
                )
                : template;
        }

        public static int CalculateStat(float statValue, float growthRate, int level)
        {
            var value = statValue * MathF.Pow(growthRate, level - 1);

            return (int)MathF.Ceiling(value);
        }
    }
}