using RTSCore.Domain.Entities;
using RTSCore.Domain.ValueObjects;
using RTSCore.Domain.ValueObjects.Configurations;

namespace RTSCore.Domain.Services;

public class TurnEndService(
    CityConfiguration cityConfiguration,
    BuildingConfiguration buildingConfiguration,
    DiplomacyConfiguration diplomacyConfiguration
)
{
    public void TurnEnd(
        Faction faction,
        IReadOnlyList<City> cities,
        IReadOnlyList<DiplomacyRelation> relations,
        IReadOnlyDictionary<FactionType, int> partnerToCityCount
    )
    {
        var citiesIncome = CitiesIncome(cities);
        var tradeIncome = TradeIncome(faction.Type, relations, partnerToCityCount);

        faction.EarnGold(citiesIncome + tradeIncome);
    }

    private int CitiesIncome(IReadOnlyList<City> cities)
    {
        int income = 0;
        for (int i = 0; i < cities.Count; i++)
        {
            cities[i].TurnEnd(
                cityConfiguration.Templates,
                buildingConfiguration.Templates,
                out int cityIncome
            );

            income += cityIncome;
        }

        return income;
    }

    private int TradeIncome(
        FactionType faction,
        IReadOnlyList<DiplomacyRelation> relations,
        IReadOnlyDictionary<FactionType, int> partnerToCityCount
    )
    {
        if (relations.Count != 0)
        {
            var relationsWithTrade = relations
                .Where(r => r.HasTradeAgreement)
                .ToList();

            int totalCities = 0;
            foreach (var relation in relationsWithTrade)
            {
                var partner = relation.FactionA == faction ? relation.FactionB : relation.FactionA;

                if (partnerToCityCount.TryGetValue(partner, out var amount))
                {
                    totalCities += amount;
                }
            }

            return totalCities * diplomacyConfiguration.TradeIncomePerPartnerCity;
        }

        return 0;
    }
}