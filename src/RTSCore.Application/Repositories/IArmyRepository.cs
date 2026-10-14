using RTSCore.Domain.Entities.Armies;

namespace RTSCore.Domain.Common.Repositories;

public interface IArmyRepository
{
    void Add(Army army);

    Task<Army?> GetAsync(ArmyId armyId, CancellationToken ct);

    Task<IReadOnlyCollection<Army>> GetFactionArmiesAsync(FactionType faction, CancellationToken ct);
}