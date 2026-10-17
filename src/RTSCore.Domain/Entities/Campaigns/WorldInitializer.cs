using RTSCore.Domain.Configurations.Cities;
using RTSCore.Domain.Configurations.Factions;
using RTSCore.Domain.Entities.Cities;
using RTSCore.Domain.Entities.Factions;

namespace RTSCore.Domain.Entities.Campaigns;

public sealed class WorldInitializer(FactionCatalog factionCatalog, CityCatalog cityCatalog)
{
    public InitializeWorldResult Initialize(CampaignId campaignId, IReadOnlySet<FactionType> playersFactions)
    {
        var campaign = new Campaign(campaignId);

        List<Faction> factions = [];
        List<City> cities = [];

        foreach (var template in factionCatalog.Templates)
        {
            var faction = playersFactions.Contains(template.Type)
                ? Faction.CreateHuman(campaignId, template)
                : Faction.CreateAi(campaignId, template, template.AiPersonalityType);

            factions.Add(faction);

            foreach (var preset in template.CityPresets)
            {
                var cityTemplate = cityCatalog.GetCity(preset.Type);
                var city = faction.FoundCity(cityTemplate, preset.Coordinates);

                cities.Add(city);
            }
        }

        return new(campaign, factions, cities);
    }
}