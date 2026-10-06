namespace RTSCore.Domain.Entities.Identity;

public interface IRefreshTokenRepository
{
    void Add(RefreshToken token);

    Task RemoveTokensAsync(Guid userId, CancellationToken cancellationToken);
    Task<RefreshToken?> GetTokenAsync(string refreshToken, CancellationToken cancellationToken);
}