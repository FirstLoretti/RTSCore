using RTSCore.Domain.Entities.Campaign;
using RTSCore.Domain.Exeptions;
using RTSCore.Domain.ValueObjects.Configurations;
using RTSCore.Domain.ValueObjects.Enums;
using RTSCore.Domain.ValueObjects.Identifiers;

namespace RTSCore.Domain.Services.Lifecycle;

public class StartCampaignService(
    FactionConfiguration factionConfiguration,
    CityConfiguration cityConfiguration
)
{
    public (IReadOnlyList<Faction> factions, IReadOnlyList<City> cities) InitializeNewWorld(
        CampaignId campaignId,
        FactionType[] humansSelectedFactions
    )
    {
        var factions = new List<Faction>();
        var cities = new List<City>();

        foreach (var template in factionConfiguration.Templates)
        {
            var isHuman = humansSelectedFactions.Contains(template.Type);
            var playerType = isHuman ? PlayerType.Human : PlayerType.Ai;

            var faction = Faction.Create(CampaignId.New(), template, playerType);
            factions.Add(faction);

            foreach (var preset in template.CityPresets)
            {
                var cityTemplate = cityConfiguration.Templates.FirstOrDefault(t => t.Type == preset.CityType)
                    ?? throw new GameRuleException("Шаблон для города не найден");

                var city = City.Create(campaignId, cityTemplate, faction.Type, preset.Coordinates);
                cities.Add(city);
            }
        }

        return (factions, cities);
    }
}