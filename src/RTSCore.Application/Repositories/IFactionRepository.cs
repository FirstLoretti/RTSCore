using RTSCore.Domain;
using RTSCore.Domain.Entities.Campaigns;
using RTSCore.Domain.Entities.Factions;

namespace RTSCore.Application.Repositories;

public interface IFactionRepository
{
    void AddRange(IEnumerable<Faction> factions);

    Task<Faction?> GetAsync(CampaignId campaignId, FactionType faction, CancellationToken ct);
}