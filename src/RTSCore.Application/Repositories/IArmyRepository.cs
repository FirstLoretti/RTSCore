using RTSCore.Domain;
using RTSCore.Domain.Entities.Armies;
using RTSCore.Domain.Entities.Campaigns;

namespace RTSCore.Application.Repositories;

public interface IArmyRepository
{
    void Add(Army army);

    Task<Army?> GetAsync(ArmyId armyId, CancellationToken ct, bool isTracking = true);
    Task<IReadOnlyList<Army>> GetAsync(
        CampaignId campaignId,
        FactionType faction,
        CancellationToken ct,
        bool isTracking = true
    );
}