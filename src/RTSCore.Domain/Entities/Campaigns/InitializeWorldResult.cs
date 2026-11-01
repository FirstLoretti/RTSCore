using System.Collections.Immutable;

using RTSCore.Domain.AI;
using RTSCore.Domain.Entities.Cities;
using RTSCore.Domain.Entities.Factions;

namespace RTSCore.Domain.Entities.Campaigns;

public readonly record struct InitializeWorldResult(
    Campaign Campaign,
    ImmutableArray<Faction> Factions,
    ImmutableArray<City> Cities,
    ImmutableArray<AiFactionAgent> AiAgents
);
