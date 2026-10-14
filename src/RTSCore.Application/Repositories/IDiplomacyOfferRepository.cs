using RTSCore.Domain.Entities.Diplomacy;

namespace RTSCore.Domain.Common.Repositories;

public interface IDiplomacyOfferRepository
{
    void Add(DiplomacyOffer offer);

    Task<DiplomacyOffer?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<HashSet<FactionType>> GetFactionsUnderNegotiationAsync(FactionType initiator, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<DiplomacyOffer>> GetFactionOffersAsync(FactionType faction, CancellationToken cancellationToken);
}