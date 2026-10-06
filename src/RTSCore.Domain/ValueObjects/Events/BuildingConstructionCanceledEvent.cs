using RTSCore.Domain.Entities.Common;
using RTSCore.Domain.ValueObjects.Enums;

namespace RTSCore.Domain.ValueObjects.Events;

public record BuildingConstructionCanceledEvent(
    int Cost,
    FactionType Faction
) : IDomainEvent;