using RTSCore.Domain.Configurations;
using RTSCore.Domain.Entities.Cities;
using RTSCore.Domain.Entities.Factions;

namespace RTSCore.Domain.Entities.Campaign;

public class StartCampaignService(GameConfigurations configurations)
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