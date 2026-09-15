using System.Collections.Frozen;

using RTSCore.Domain.Exeptions;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Domain.Services;

public partial class GameBalance
{
    public static class Cities
    {
        private static readonly FrozenDictionary<CityType, CityTemplate> TypeToTemplate;

        static Cities()
        {
            Dictionary<CityType, CityTemplate> temporary = new()
            {
                {
                    CityType.Village,
                    new CityTemplate(
                        DisplayName: "Деревня",
                        Type: CityType.Village,
                        MaxPopulation: 1000,
                        BuildingOptions :
                        [BuildingType.CultivatedField, BuildingType.ReqruitBarrack, BuildingType.Market]
                    )
                },

                {
                    CityType.Settlement,
                    new CityTemplate(
                        DisplayName : "Посёлок",
                        Type: CityType.Settlement,
                        MaxPopulation: 3000,
                        BuildingOptions:
                        [BuildingType.CultivatedField, BuildingType.ReqruitBarrack, BuildingType.Market,
                        BuildingType.CultivatedFieldsComplex, BuildingType.MilitiaBarrack]
                    )
                }
            };

            TypeToTemplate = temporary.ToFrozenDictionary();
        }

        public static CityTemplate GetCityTemplate(CityType type)
        {
            return !TypeToTemplate.TryGetValue(type, out var template)
                ? throw new ArgumentException(
                    $"[{nameof(Cities)}] Шаблон города {template} не найден в системе"
                )
                : template;
        }

        public static List<BuildingType> GetBuildingOptions(CityType type)
        {
            return TypeToTemplate.TryGetValue(type, out var template)
                ? template.BuildingOptions
                : throw new NotFoundException($"Тип города {type} не найден");
        }
    }
}