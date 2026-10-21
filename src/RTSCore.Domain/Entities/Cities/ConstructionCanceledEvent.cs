using RTSCore.Domain.Common;

namespace RTSCore.Domain.Entities.Cities;

public record ConstructionCanceledEvent(
    FactionType Faction,
    int Cost
) : IDomainEvent;