using Microsoft.EntityFrameworkCore;

using RTSCore.Domain.Entities;
using RTSCore.Domain.Interfaces;

namespace RTSCore.Infrastructure.Persistence;

public class SqlUserRepository(AppDbContext context) : IUserRepository
{
    public void Add(User user)
    {
        context.Users.Add(user);
    }

    public async Task<bool> ExistAsync(string name, CancellationToken cancellationToken)
    {
        return await context.Users.AnyAsync(u => u.Name == name, cancellationToken);
    }

    public async Task<User?> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.Name == name, cancellationToken);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }
}