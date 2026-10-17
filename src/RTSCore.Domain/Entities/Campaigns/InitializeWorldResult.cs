using RTSCore.Domain.Entities.Cities;
using RTSCore.Domain.Entities.Factions;

namespace RTSCore.Domain.Entities.Campaigns;

public readonly record struct InitializeWorldResult(
    Campaign Campaign,
    IReadOnlyCollection<Faction> Factions,
    IReadOnlyCollection<City> Cities
);
