using RTSCore.Domain.Entities.Diplomacy;

namespace RTSCore.Application.Repositories;

public interface IDiplomacyOfferRepository
{
    void Add(DiplomacyOffer offer);

    Task<DiplomacyOffer?> GetAsync(DiplomacyOfferId id, CancellationToken ct);
}