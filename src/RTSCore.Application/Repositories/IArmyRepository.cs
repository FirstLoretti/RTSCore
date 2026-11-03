using System.Collections.ObjectModel;

using RTSCore.Domain;
using RTSCore.Domain.Entities.Armies;

namespace RTSCore.Application.Repositories;

public interface IArmyRepository
{
    void Add(Army army);

    Task<Army?> GetAsync(ArmyId armyId, CancellationToken ct);
    Task<ReadOnlyCollection<Army>> GetAsync(FactionType faction, CancellationToken ct);
}