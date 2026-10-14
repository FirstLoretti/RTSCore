using RTSCore.Domain.Common;

namespace RTSCore.Domain.Entities.Cities;

public record BuildingConstructionCanceledEvent(
    int Cost,
    FactionType Faction
) : IDomainEvent;