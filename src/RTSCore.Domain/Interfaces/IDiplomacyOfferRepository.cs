using RTSCore.Domain.Entities;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Domain.Interfaces;

public interface IDiplomacyOfferRepository
{
    void Add(DiplomacyOffer offer);

    Task<DiplomacyOffer?> GetOfferAsync(Guid id, CancellationToken cancellationToken);
    Task<HashSet<FactionType>> GetFactionsUnderNegotiationAsync(FactionType initiator, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<DiplomacyOffer>> GetFactionOffersAsync(FactionType faction, CancellationToken cancellationToken);
}