using RTSCore.Domain.Common;

namespace RTSCore.Domain.Entities.Cities;

public record BuildingConstructionCanceledEvent(
    FactionType Faction,
    int Cost
) : IDomainEvent;