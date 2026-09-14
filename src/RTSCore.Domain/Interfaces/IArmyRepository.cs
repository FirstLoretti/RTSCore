using RTSCore.Domain.Entities;

namespace RTSCore.Domain.Interfaces;

public interface IArmyRepository
{
    void Add(Army army);

    Task<Army?> GetAsync(string armyId);
}