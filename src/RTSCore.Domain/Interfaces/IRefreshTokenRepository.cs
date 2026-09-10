using RTSCore.Domain.Entities;

namespace RTSCore.Domain.Interfaces;

public interface IRefreshTokenRepository
{
    void Add(RefreshToken token);

    Task RemoveTokensAsync(Guid userId, CancellationToken cancellationToken);
    Task<RefreshToken?> GetTokenAsync(string refreshToken, CancellationToken cancellationToken);
}