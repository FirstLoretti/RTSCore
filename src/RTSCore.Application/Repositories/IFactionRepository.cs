using RTSCore.Domain.Entities.Campaign;
using RTSCore.Domain.Enums;

namespace RTSCore.Domain.Common.Repositories;

public interface IFactionRepository
{
    void Add(Faction faction);
    void Remove(Faction faction);
    void AddRange(IEnumerable<Faction> factions);

    Task<Faction?> GetAsync(FactionType factionType, CancellationToken ct);
    Task<IReadOnlyCollection<Faction>> GetAsync(CancellationToken ct);
    Task<bool> HasAnyAsync(CancellationToken ct);
}