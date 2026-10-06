using RTSCore.Domain.ValueObjects.Enums;

namespace RTSCore.Domain.Entities.Campaign.Diplomacy;

public interface IDiplomacyOfferRepository
{
    void Add(DiplomacyOffer offer);

    Task<DiplomacyOffer?> GetOfferAsync(Guid id, CancellationToken cancellationToken);
    Task<HashSet<FactionType>> GetFactionsUnderNegotiationAsync(FactionType initiator, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<DiplomacyOffer>> GetFactionOffersAsync(FactionType faction, CancellationToken cancellationToken);
}