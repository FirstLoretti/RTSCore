using RTSCore.Domain.ValueObjects.Enums;
using RTSCore.Domain.ValueObjects.Identifiers;
using RTSCore.Domain.ValueObjects.Templates;

namespace RTSCore.Domain.Entities.Campaign;

public sealed class Building
{
    public CampaignId CampaignId { get; }

    public BuildingId Id { get; }
    public BuildingType Type { get; }
    public FactionType Faction { get; }
    public CityId CityId { get; }
    public int Cost { get; }
    public int TurnsToConstruct { get; private set; }

    public bool IsConstructed => TurnsToConstruct <= 0;

    private Building(
        CampaignId campaignId,
        BuildingId buildingId,
        BuildingTemplate template,
        FactionType faction,
        CityId cityId,
        int turnsToConstruct
    )
    {
        CampaignId = campaignId;
        Id = buildingId;
        Type = template.Type;
        Faction = faction;
        CityId = cityId;
        TurnsToConstruct = turnsToConstruct;
        Cost = template.Cost;
    }

    private Building() { }

    internal static Building CreateUnderConstruction(
        CampaignId campaignId,
        BuildingTemplate template,
        FactionType faction,
        CityId cityId
    ) => new(campaignId, BuildingId.New(), template, faction, cityId, template.TurnsToConstruct);

    internal static Building CreateConstructed(
        CampaignId campaignId,
        BuildingTemplate template,
        FactionType faction,
        CityId cityId
    ) => new(campaignId, BuildingId.New(), template, faction, cityId, turnsToConstruct: 0);

    internal void AdvanceConstruction() => TurnsToConstruct--;
}