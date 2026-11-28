using RTSCore.Domain.Configurations.Armies;
using RTSCore.Domain.Entities.Armies;
using RTSCore.Domain.Entities.Cities;
using RTSCore.Domain.Entities.Diplomacy;
using RTSCore.Domain.Entities.Factions;

namespace RTSCore.Domain.Entities.Campaigns;

public sealed class CampaignWorldState
{
    private readonly Dictionary<FactionType, Faction> _factionsByType;
    private readonly Dictionary<FactionType, FactionContext> _factionContextByFaction = [];
    private readonly Dictionary<FactionType, List<City>> _citiesByFaction;
    private readonly Dictionary<FactionType, List<Army>> _armiesByFaction;
    private readonly Dictionary<FactionType, List<DiplomacyRelation>> _relationsByFaction;
    private readonly Dictionary<(FactionType, FactionType), DiplomacyRelation> _relationByPair;
    private readonly Dictionary<FactionType, List<DiplomacyOffer>> _pendingOffersByFaction;
    private readonly List<DiplomacyOffer> _createdOffers = [];

    public Faction GetFaction(FactionType faction) => _factionsByType[faction];
    public FactionContext GetFactionContext(FactionType faction) => _factionContextByFaction[faction];
    public DiplomacyRelation GetRelation(FactionType factionA, FactionType factionB) => _relationByPair[(factionA, factionB)];
    public IReadOnlyCollection<FactionContext> AllFactionContexts => _factionContextByFaction.Values;
    public IReadOnlyCollection<Faction> AllFactions => _factionsByType.Values;
    public IReadOnlyList<City> GetCities(FactionType faction) => _citiesByFaction[faction];
    public IReadOnlyList<Army> GetArmies(FactionType faction) => _armiesByFaction[faction];
    public IReadOnlyList<DiplomacyRelation> GetRelations(FactionType faction) => _relationsByFaction[faction];
    public IReadOnlyList<DiplomacyOffer> GetPendingOffers(FactionType faction) => _pendingOffersByFaction[faction];
    public IReadOnlyList<DiplomacyOffer> CreatedOffers => _createdOffers;


    public CampaignWorldState(
        IReadOnlyCollection<Faction> factions,
        IReadOnlyCollection<City> cities,
        IReadOnlyCollection<Army> armies,
        IReadOnlyCollection<DiplomacyRelation> relations,
        IReadOnlyCollection<DiplomacyOffer> pendingOffers,
        UnitStatsCalculator statsCalculator,
        UnitCatalog unitCatalog
    )
    {
        _factionsByType = factions.ToDictionary(f => f.Type);
        _citiesByFaction = factions.ToDictionary(f => f.Type, _ => new List<City>());
        _armiesByFaction = factions.ToDictionary(f => f.Type, _ => new List<Army>());
        _relationsByFaction = factions.ToDictionary(f => f.Type, _ => new List<DiplomacyRelation>());
        _pendingOffersByFaction = factions.ToDictionary(f => f.Type, _ => new List<DiplomacyOffer>());

        foreach (var city in cities) _citiesByFaction[city.Faction].Add(city);
        foreach (var army in armies) _armiesByFaction[army.Faction].Add(army);
        foreach (var offer in pendingOffers) _pendingOffersByFaction[offer.Target].Add(offer);
        foreach (var faction in factions) RefreshFactionContext(faction.Type, statsCalculator, unitCatalog);

        _relationByPair = new(relations.Count * 2);
        foreach (var relation in relations)
        {
            _relationsByFaction[relation.FactionA].Add(relation);
            _relationsByFaction[relation.FactionB].Add(relation);

            _relationByPair[(relation.FactionA, relation.FactionB)] = relation;
            _relationByPair[(relation.FactionB, relation.FactionA)] = relation;
        }
    }

    public void AddNewOffer(DiplomacyOffer offer)
    {
        _createdOffers.Add(offer);
        _pendingOffersByFaction[offer.Target].Add(offer);
    }

    public void RefreshFactionContext(FactionType factionType, UnitStatsCalculator statsCalculator, UnitCatalog unitCatalog)
    {
        var faction = GetFaction(factionType);
        var cities = GetCities(factionType);
        var armies = GetArmies(factionType);
        var power = armies
            .SelectMany(a => a.GetReadyCombatants())
            .Sum(u =>
            {
                var template = unitCatalog.GetTemplate(u.Type);
                return statsCalculator.Power(template, u.Level, u.Health);
            });

        _factionContextByFaction[factionType] = new FactionContext(factionType, faction.Gold, cities.Count, power);
    }
}