using RTSCore.Domain.Entities.Campaign;
using RTSCore.Domain.Enums;
using RTSCore.Domain.ValueObjects.Identifiers;

namespace RTSCore.Domain.Common.Repositories;

public interface IArmyRepository
{
    void Add(Army army);

    Task<Army?> GetAsync(ArmyId armyId, CancellationToken ct);

    Task<IReadOnlyCollection<Army>> GetFactionArmiesAsync(FactionType faction, CancellationToken ct);
}