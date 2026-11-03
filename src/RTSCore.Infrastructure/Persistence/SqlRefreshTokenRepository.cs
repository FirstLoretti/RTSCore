using Microsoft.EntityFrameworkCore;

using RTSCore.Application;
using RTSCore.Application.Authentication;
using RTSCore.Application.Repositories;

namespace RTSCore.Infrastructure.Persistence;

public sealed class SqlRefreshTokenRepository(AppDbContext context) : IRefreshTokenRepository
{
    public void Add(RefreshToken token) => context.RefreshTokens.Add(token);

    public async Task RemoveTokensAsync(UserId userId, CancellationToken ct) =>
        await context.RefreshTokens
            .Where(t => t.UserId == userId)
            .ExecuteDeleteAsync(ct);

    public async Task<RefreshToken?> GetAsync(string token, CancellationToken ct) =>
        await context.RefreshTokens.FindAsync([token], ct);
}