using System.Collections.Frozen;
using System.Collections.Immutable;

using RTSCore.Domain.ValueObjects;

namespace RTSCore.Domain.Services;

public static partial class GameBalance
{
    public static class Units
    {
        public static readonly ImmutableArray<int> ExpToNextLevel = [50, 100, 150, 200];

        private static readonly FrozenDictionary<UnitType, UnitTemplate> TypeToTemplate;

        static Units()
        {
            Dictionary<UnitType, UnitTemplate> temporary = new()
            {
                {
                    UnitType.EnglandSwordman,
                    new UnitTemplate(
                        Type: UnitType.EnglandSwordman,
                        DisplayName: "EnglandSwordman",
                        MaxHealth: 100,
                        Damage: 25,
                        Armor: 2,
                        Speed: 5,
                        ExpKillReward: 50,
                        HealthGrowthRate: 1.1f,
                        DamageGrowthRate: 1.15f)
                },
                {
                    UnitType.FranceSwordman,
                    new UnitTemplate(
                        Type: UnitType.FranceSwordman,
                        DisplayName: "FranceSwordman",
                        MaxHealth: 115,
                        Damage: 20,
                        Armor: 3,
                        Speed: 5,
                        ExpKillReward: 50,
                        HealthGrowthRate: 1.15f,
                        DamageGrowthRate: 1.10f)
                },
                {
                    UnitType.Invulnerable,
                    new UnitTemplate(
                        Type: UnitType.Invulnerable,
                        DisplayName: "Invulnerable",
                        MaxHealth: 1,
                        Damage: 1,
                        Armor: 1,
                        Speed: 1,
                        ExpKillReward: 1,
                        HealthGrowthRate: 1f,
                        DamageGrowthRate: 1f)
                }
            };

            TypeToTemplate = temporary.ToFrozenDictionary();
        }

        public static UnitTemplate GetTemplate(UnitType type)
        {
            return !TypeToTemplate.TryGetValue(type, out var template)
                ? throw new ArgumentException(
                    $"[GameBalance.Units] Шаблон юнита {type} не найден в системе"
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