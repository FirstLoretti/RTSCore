using RTSCore.Domain.Interfaces;

namespace RTSCore.Infrastructure.Persistence;

public class EfUnitOfWork(AppDbContext context) : IUnitOfWork
{
    public IBuildingRepository BuildingRepository => new SqlBuildingRepository(context);

    public IUnitRepository UnitRepository => new SqlUnitRepository(context);

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}