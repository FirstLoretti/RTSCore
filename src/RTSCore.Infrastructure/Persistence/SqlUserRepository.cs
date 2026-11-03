using Microsoft.EntityFrameworkCore;

using RTSCore.Application;
using RTSCore.Application.Repositories;

namespace RTSCore.Infrastructure.Persistence;

public sealed class SqlUserRepository(AppDbContext context) : IUserRepository
{
    public void Add(User user) => context.Users.Add(user);

    public async Task<bool> ExistAsync(string name, CancellationToken ct) =>
        await context.Users.AnyAsync(u => u.Name == name, ct);

    public async Task<User?> GetAsync(string name, CancellationToken ct) =>
        await context.Users.FindAsync([name], ct);

    public async Task<User?> GetAsync(UserId id, CancellationToken ct) =>
        await context.Users.FindAsync([id], ct);
}