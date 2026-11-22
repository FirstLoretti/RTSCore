using System.Collections.Immutable;

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
        IReadOnlyCollection<FactionType> playersFactions,
        FactionCatalog factionCatalog,
        CityCatalog cityCatalog
    )
    {
        var campaign = new Campaign(campaignId);
        var totalFactions = factionCatalog.AllTemplates.Length;

        var factions = new List<Faction>(totalFactions);
        var aiAgents = new List<AiFactionAgent>(totalFactions - playersFactions.Count);
        var cities = new List<City>();

        foreach (var template in factionCatalog.AllTemplates)
        {
            var faction = new Faction(campaignId, template);
            factions.Add(faction);

            if (!playersFactions.Contains(template.Type))
            {
                aiAgents.Add(new(campaignId, template.Type, template.AiPersonalityType));
            }

            foreach (var preset in template.CityPresets)
            {
                var cityTemplate = cityCatalog.GetTemplate(preset.Type);
                var city = faction.FoundCity(cityTemplate, preset.Coordinates);

                cities.Add(city);
            }
        }

        return new(campaign, factions, cities, aiAgents);
    }
}