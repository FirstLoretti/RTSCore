using RTSCore.Domain.AI;
using RTSCore.Domain.Configurations.Cities;
using RTSCore.Domain.Configurations.Factions;
using RTSCore.Domain.Entities.Cities;
using RTSCore.Domain.Entities.Factions;

namespace RTSCore.Domain.Entities.Campaigns;

public static class WorldInitializer
{
    public static InitializeWorldResult Initialize(
        CampaignId campaignId,
        IReadOnlySet<FactionType> playersFactions,
        FactionCatalog factionCatalog,
        CityCatalog cityCatalog
    )
    {
        var campaign = new Campaign(campaignId);

        List<Faction> factions = [];
        List<City> cities = [];
        List<AiFactionAgent> aiAgents = [];

        foreach (var template in factionCatalog.Templates)
        {
            var faction = new Faction(campaignId, template);
            factions.Add(faction);

            if (!playersFactions.Contains(template.Type))
            {
                aiAgents.Add(new(campaignId, template.Type, template.AiPersonalityType));
            }

            foreach (var preset in template.CityPresets)
            {
                var cityTemplate = cityCatalog.GetCity(preset.Type);
                var city = faction.FoundCity(cityTemplate, preset.Coordinates);

                cities.Add(city);
            }
        }

        return new(campaign, factions, cities, aiAgents);
    }
}