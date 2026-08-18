using System.Collections.Frozen;

using RTSCore.Domain.ValueObjects;

namespace RTSCore.Domain.Services;

public partial class GameBalance
{
    public static class Buildings
    {
        public static readonly FrozenDictionary<BuildingType, BuildingTemplate> TypeToTemplate;

        static Buildings()
        {
            Dictionary<BuildingType, BuildingTemplate> temporary = new()
            {
                {
                    BuildingType.EnglandBarrack,
                    new BuildingTemplate(
                        Type: BuildingType.EnglandBarrack,
                        Faction: FactionType.England,
                        DisplayName: "England Barrack",
                        MaxHealth: 1000,
                        MaxRecruitmentSlots : 1
                    )
                },

                {
                    BuildingType.FranceBarrack,
                    new BuildingTemplate(
                        Type: BuildingType.FranceBarrack,
                        Faction: FactionType.France,
                        DisplayName: "France Barrack",
                        MaxHealth: 1250,
                        MaxRecruitmentSlots : 1
                    )
                }
            };

            TypeToTemplate = temporary.ToFrozenDictionary();
        }

        public static BuildingTemplate GetTemplate(BuildingType type)
        {
            return !TypeToTemplate.TryGetValue(type, out var template)
                ? throw new ArgumentException(
                    $"[GameBalance.Buildings] Шаблон здания {type} не найдён в системе"
                )
                : template;
        }
    }
}