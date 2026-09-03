using RTSCore.Domain.Entities;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Domain.Interfaces;

public interface IFactionRepository
{
    void Add(Faction faction);
    void Remove(Faction faction);
    void AddRange(IEnumerable<Faction> factions);

    Task<Faction?> GetFactionAsync(FactionType faction, CancellationToken cancellationToken);
    Task<bool> HasAnyAsync(CancellationToken cancellationToken);
    Task<IReadOnlyCollection<FactionType>> GetAnotherFactionsAsync(FactionType currentFaction, CancellationToken cancellationToken);
}