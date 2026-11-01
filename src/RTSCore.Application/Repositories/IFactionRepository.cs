using RTSCore.Domain;
using RTSCore.Domain.Entities.Factions;

namespace RTSCore.Application.Repositories;

public interface IFactionRepository
{
    void AddRange(IEnumerable<Faction> factions);

    Task<Faction?> GetAsync(FactionType faction, CancellationToken ct);
}