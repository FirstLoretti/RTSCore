using Microsoft.EntityFrameworkCore;

using RTSCore.Application;
using RTSCore.Application.Repositories;

namespace RTSCore.Infrastructure.Persistence;

public sealed class UserRepository(AppDbContext context) : IUserRepository
{
    public void Add(User user) => context.Users.Add(user);

    public async Task<bool> ExistAsync(string name, CancellationToken ct) =>
        await context.Users.AnyAsync(u => u.Name == name, ct);

    public async Task<User?> GetAsync(string name, CancellationToken ct, bool isTracking = true) =>
        await context.Users
            .WithTracking(isTracking)
            .FirstOrDefaultAsync(u => u.Name == name, ct);

    public async Task<User?> GetAsync(UserId id, CancellationToken ct, bool isTracking = true) =>
        await context.Users
            .WithTracking(isTracking)
            .FirstOrDefaultAsync(u => u.Id == id);
}