using RTSCore.Domain.AI;
using RTSCore.Domain.Entities.Armies;
using RTSCore.Domain.Entities.Cities;
using RTSCore.Domain.Entities.Diplomacy;
using RTSCore.Domain.Entities.Factions;

namespace RTSCore.Domain.Entities.Campaigns;

public readonly record struct InitializeWorldResult(
    Campaign Campaign,
    IReadOnlyList<Faction> Factions,
    IReadOnlyList<City> Cities,
    IReadOnlyList<Army> Armies,
    IReadOnlyList<DiplomacyRelation> DiplomacyRelations,
    IReadOnlyList<AiFactionAgent> AiAgents
);
