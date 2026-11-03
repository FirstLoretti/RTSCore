using RTSCore.Application.Authentication;

namespace RTSCore.Application.Repositories;

public interface IRefreshTokenRepository
{
    void Add(RefreshToken token);
    Task RemoveTokensAsync(UserId userId, CancellationToken ct);

    Task<RefreshToken?> GetAsync(string token, CancellationToken ct);
}