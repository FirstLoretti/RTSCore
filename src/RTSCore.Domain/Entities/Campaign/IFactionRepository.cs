using RTSCore.Domain.ValueObjects.Enums;

namespace RTSCore.Domain.Entities.Campaign;

public interface IFactionRepository
{
    void Add(Faction faction);
    void Remove(Faction faction);
    void AddRange(IEnumerable<Faction> factions);

    Task<Faction?> GetFactionAsync(FactionType faction, CancellationToken cancellationToken);
    Task<bool> HasAnyAsync(CancellationToken cancellationToken);

    Task<IReadOnlyCollection<FactionType>> GetAnotherFactionsAsync(
        FactionType currentFaction, CancellationToken cancellationToken
    );

    Task<Dictionary<FactionType, int>> GetFactionToMilitaryPower(
        IEnumerable<FactionType> factions, CancellationToken cancellationToken
    );
}