using RTSCore.Domain.Interfaces;

namespace RTSCore.Domain.ValueObjects.Events;

public record BuildingConstructionCanceledEvent(
    int Cost,
    FactionType Faction
) : IDomainEvent;