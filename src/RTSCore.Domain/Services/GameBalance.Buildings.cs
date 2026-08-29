using System.Collections.Frozen;

using RTSCore.Domain.ValueObjects;

namespace RTSCore.Domain.Services;

public partial class GameBalance
{
    public static class Buildings
    {
        private static readonly FrozenDictionary<BuildingType, BuildingTemplate> TypeToTemplate;

        public static IReadOnlyCollection<BuildingTemplate> AllTemplates => TypeToTemplate.Values;

        static Buildings()
        {
            Dictionary<BuildingType, BuildingTemplate> temporary = new()
            {
                {
                    BuildingType.ReqruitBarrack,
                    new BuildingTemplate(
                        Type: BuildingType.ReqruitBarrack,
                        DisplayName: "Казарма",
                        Cost: 1000,
                        TurnsToConstruct: 2,
                        AllowedCityTypes: [CityType.Settlement,CityType.Village]
                    )
                },

                {
                    BuildingType.MilitiaBarrack,
                    new BuildingTemplate(
                        Type: BuildingType.MilitiaBarrack,
                        DisplayName: "Казарма Ополченцев",
                        Cost: 3000,
                        TurnsToConstruct: 4,
                        AllowedCityTypes: [CityType.Settlement],
                        RequiredPreviousTier: BuildingType.ReqruitBarrack
                    )
                },

                {
                    BuildingType.Market,
                    new BuildingTemplate(
                        Type: BuildingType.Market,
                        DisplayName: "Рынок",
                        Cost: 1500,
                        TurnsToConstruct: 2,
                        AllowedCityTypes: [CityType.Settlement,CityType.Village]
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