using System.Collections.Frozen;

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
                        MaxPopulation: 1000
                    )
                },

                {
                    CityType.Town,
                    new CityTemplate(
                        DisplayName : "Город",
                        Type: CityType.Town,
                        MaxPopulation: 4000
                    )
                },

                {
                    CityType.WoodenCastle,
                    new CityTemplate(
                        DisplayName: "Деревянный замок",
                        Type: CityType.WoodenCastle,
                        MaxPopulation: 500
                    )
                },

                {
                    CityType.StoneCastle,
                    new CityTemplate(
                        DisplayName: "Каменный замок",
                        Type: CityType.StoneCastle,
                        MaxPopulation: 1500
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
    }
}