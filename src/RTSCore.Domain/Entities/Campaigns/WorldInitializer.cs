using System.Collections.Immutable;

using RTSCore.Domain.AI;
using RTSCore.Domain.Configurations.Cities;
using RTSCore.Domain.Configurations.Factions;
using RTSCore.Domain.Entities.Armies;
using RTSCore.Domain.Entities.Cities;
using RTSCore.Domain.Entities.Diplomacy;
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
        var factionsTemplates = factionCatalog.AllTemplates;
        var campaign = new Campaign(campaignId, factionsTemplates.Select(f => f.Type), playersFactions);

        var factions = new List<Faction>(factionsTemplates.Length);
        var aiAgents = new List<AiFactionAgent>(factionsTemplates.Length - playersFactions.Count);
        var cities = new List<City>();
        var armies = new List<Army>();

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
                var city = City.Create(faction.CampaignId, cityTemplate, faction.Type, preset.Coordinates);

                foreach (var building in preset.InitialBuildings)
                {
                    city.ConstructInstantly(building);
                }

                cities.Add(city);
            }

            foreach (var preset in template.ArmyPresets)
            {
                var army = Army.Create(faction.CampaignId, faction.Type, preset.Coordinates, preset.General);

                foreach (var unit in preset.Units)
                {
                    army.AddReadyUnit(unit);
                }

                armies.Add(army);
            }
        }

        return new(campaign, factions, cities, armies, InitializeRelations(factions), aiAgents);
    }

    private static List<DiplomacyRelation> InitializeRelations(List<Faction> factions)
    {
        var pairsCount = factions.Count * (factions.Count - 1);
        var relations = new List<DiplomacyRelation>(pairsCount);

        for (int i = 0; i < factions.Count; i++)
        {
            for (int j = i + 1; j < factions.Count; j++)
            {
                var relation = new DiplomacyRelation(factions[0].CampaignId, factions[i].Type, factions[j].Type, standing: 0);
                relations.Add(relation);
            }
        }

        return relations;
    }
}