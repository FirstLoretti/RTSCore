using Microsoft.EntityFrameworkCore;

using RTSCore.Domain.Entities;
using RTSCore.Domain.Interfaces;

namespace RTSCore.Infrastructure.Persistence;

public class SqlRefreshTokenRepository(AppDbContext context) : IRefreshTokenRepository
{
    public void Add(RefreshToken token) => context.RefreshTokens.Add(token);

    public async Task RemoveTokensAsync(Guid userId, CancellationToken cancellationToken)
    {
        await context.RefreshTokens
                .Where(t => t.UserId == userId)
                .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<RefreshToken?> GetTokenAsync(string refreshToken, CancellationToken cancellationToken)
    {
        return await context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == refreshToken, cancellationToken);
    }
}