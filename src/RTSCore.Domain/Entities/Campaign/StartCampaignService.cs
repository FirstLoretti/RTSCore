using RTSCore.Domain.Configurations;
using RTSCore.Domain.Entities.Campaign;
using RTSCore.Domain.Enums;
using RTSCore.Domain.ValueObjects.Identifiers;

namespace RTSCore.Domain.Services.Lifecycle;

public class StartCampaignService(GameConfiguations configurations)
{
    public (IReadOnlyCollection<Faction> factions, IReadOnlyCollection<City> cities) InitializeWorld(
        CampaignId campaignId,
        FactionType[] playersFactions
    )
    {
        var cityTypeToTemplate = configurations.Cities.Templates.ToDictionary(t => t.Type);

        List<Faction> factions = [];
        List<City> cities = [];

        foreach (var template in configurations.Factions.Templates)
        {
            var faction = playersFactions.Contains(template.Type)
                ? Faction.CreateHuman(campaignId, template)
                : Faction.CreateAi(campaignId, template, template.AiPersonalityType);

            factions.Add(faction);

            foreach (var preset in template.CityPresets)
            {
                var cityTemplate = cityTypeToTemplate[preset.Type];
                var city = faction.FoundCity(cityTemplate, preset.Coordinates);

                cities.Add(city);
            }
        }

        return (factions, cities);
    }
}