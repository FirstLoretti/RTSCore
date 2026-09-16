using RTSCore.Domain.Entities;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Domain.Interfaces;

public interface IArmyRepository
{
    void Add(Army army);

    Task<Army?> GetAsync(string armyId, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Army>> GetArmiesAsync(FactionType faction, CancellationToken cancellationToken);
}