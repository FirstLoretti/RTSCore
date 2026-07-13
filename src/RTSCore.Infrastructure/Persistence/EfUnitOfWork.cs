using RTSCore.Domain.Interfaces;

namespace RTSCore.Infrastructure.Persistence;

public class EfUnitOfWork(AppDbContext context) : IUnitOfWork
{
    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}