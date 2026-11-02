namespace RTSCore.Application.Authentication;

public sealed class RefreshToken
{
    public RefreshTokenId Id { get; }
    public string Token { get; } = string.Empty;
    public UserId UserId { get; }
    public DateTime ExpiryTime { get; }

    public bool IsUsed { get; private set; }
    public bool IsRevoked { get; private set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiryTime;

    public RefreshToken(UserId userId, string token, DateTime expiryTime)
    {
        Id = RefreshTokenId.New();
        UserId = userId;
        Token = token;
        ExpiryTime = expiryTime;
    }

    private RefreshToken() { }

    public void Use() => IsUsed = true;
    public void Revoke() => IsRevoked = true;
}