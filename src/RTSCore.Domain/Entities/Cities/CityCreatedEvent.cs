using RTSCore.Domain.Enums;

namespace RTSCore.Domain.Events;

public record CityCreatedEvent(
    FactionType Faction
) : IDomainEvent;