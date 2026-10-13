using RTSCore.Domain.Enums;

namespace RTSCore.Domain.Events;

public record BuildingConstructionCanceledEvent(
    int Cost,
    FactionType Faction
) : IDomainEvent;