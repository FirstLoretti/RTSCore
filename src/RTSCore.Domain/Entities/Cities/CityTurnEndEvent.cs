using RTSCore.Domain.Common;

namespace RTSCore.Domain.Entities.Cities;

public record CityTurnEndEvent(
    FactionType Faction,
    int Income
) : IDomainEvent;