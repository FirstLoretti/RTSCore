using RTSCore.Domain.Common;

namespace RTSCore.Domain.Entities.Cities;

public record CityTurnEndEvent(
    FactionType Faction,
    int BuildingsIncome,
    int TaxesIncome
) : IDomainEvent;