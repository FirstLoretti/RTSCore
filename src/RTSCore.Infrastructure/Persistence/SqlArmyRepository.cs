using Microsoft.EntityFrameworkCore;

using RTSCore.Domain.Entities;
using RTSCore.Domain.Interfaces;

namespace RTSCore.Infrastructure.Persistence;

public class SqlArmyRepository(AppDbContext context) : IArmyRepository
{
    public void Add(Army army) => context.Armies.Add(army);

    public async Task<Army?> GetAsync(string armyId) =>
        await context.Armies.FirstOrDefaultAsync(a => a.Id == armyId);
}