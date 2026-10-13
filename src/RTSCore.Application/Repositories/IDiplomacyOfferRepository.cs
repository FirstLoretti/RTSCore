using RTSCore.Domain.Entities.Campaign;
using RTSCore.Domain.Enums;

namespace RTSCore.Domain.Common.Repositories;

public interface IDiplomacyOfferRepository
{
    void Add(DiplomacyOffer offer);

    Task<DiplomacyOffer?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<HashSet<FactionType>> GetFactionsUnderNegotiationAsync(FactionType initiator, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<DiplomacyOffer>> GetFactionOffersAsync(FactionType faction, CancellationToken cancellationToken);
}