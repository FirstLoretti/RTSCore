using RTSCore.Domain.AI;
using RTSCore.Domain.Entities.Cities;
using RTSCore.Domain.Entities.Factions;

namespace RTSCore.Domain.Entities.Campaigns;

public readonly record struct InitializeWorldResult(
    Campaign Campaign,
    IReadOnlyList<Faction> Factions,
    IReadOnlyList<City> Cities,
    IReadOnlyList<AiFactionAgent> AiAgents
);
