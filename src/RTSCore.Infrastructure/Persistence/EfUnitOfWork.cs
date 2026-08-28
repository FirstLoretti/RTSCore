using RTSCore.Domain.Interfaces;

namespace RTSCore.Infrastructure.Persistence;

public class EfUnitOfWork(AppDbContext context) : IUnitOfWork
{
    public IBuildingRepository BuildingRepository { get; } = new SqlBuildingRepository(context);
    public IUnitRepository UnitRepository { get; } = new SqlUnitRepository(context);
    public ICityRepository CityRepository { get; } = new SqlCityRepository(context);
    public IFactionRepository FactionRepository { get; } = new SqlFactionRepository(context);

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}