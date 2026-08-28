using System.Collections.Frozen;

using RTSCore.Domain.ValueObjects;

namespace RTSCore.Domain.Services;

public partial class GameBalance
{
    public static class Buildings
    {
        private static readonly FrozenDictionary<BuildingType, BuildingTemplate> TypeToTemplate;

        static Buildings()
        {
            Dictionary<BuildingType, BuildingTemplate> temporary = new()
            {
                {
                    BuildingType.Barrack,
                    new BuildingTemplate(
                        Type: BuildingType.Barrack,
                        DisplayName: "Казарма",
                        Cost: 1000,
                        1
                    )
                },

                {
                    BuildingType.Market,
                    new BuildingTemplate(
                        Type: BuildingType.Market,
                        DisplayName: "Рынок",
                        Cost: 1500,
                        2
                    )
                }
            };

            TypeToTemplate = temporary.ToFrozenDictionary();
        }

        public static BuildingTemplate GetTemplate(BuildingType type)
        {
            return !TypeToTemplate.TryGetValue(type, out var template)
                ? throw new ArgumentException(
                    $"[{nameof(Buildings)}] Шаблон здания {type} не найдён в системе"
                )
                : template;
        }
    }
}