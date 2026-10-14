using RTSCore.Domain.Common;

namespace RTSCore.Domain.Entities.Cities;

public record CityCreatedEvent(
    FactionType Faction
) : IDomainEvent;