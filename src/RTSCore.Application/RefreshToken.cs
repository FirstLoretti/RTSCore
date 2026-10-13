namespace RTSCore.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; init; }
    public string Token { get; init; } = string.Empty;
    public Guid UserId { get; init; }
    public DateTime ExpiryTime { get; init; }
    public bool IsUsed { get; private set; }
    public bool IsRevoked { get; private set; }

    public bool IsExpired => DateTime.UtcNow > ExpiryTime;
    public bool IsActive => !IsExpired && !IsUsed && !IsRevoked;

    public RefreshToken(Guid userId, string token, DateTime expiryTime)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Token = token;
        ExpiryTime = expiryTime;
    }

    private RefreshToken() { }

    public void Use() => IsUsed = true;
    public void Revoke() => IsRevoked = true;
}