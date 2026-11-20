using RTSCore.Domain.Common;

namespace RTSCore.Application.Authentication;

public sealed class RefreshToken
{
    public RefreshTokenId Id { get; }
    public string Token { get; } = string.Empty;
    public UserId UserId { get; }
    public DateTime ExpiryTime { get; init; }

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

    public Result Revoke(UserId userId)
    {
        if (UserId != userId) return AuthErrors.RefreshTokenCompromized;

        IsRevoked = true;

        return Result.Success();
    }

    public Result Use(UserId userId)
    {
        if (UserId != userId || IsUsed || IsRevoked) return AuthErrors.RefreshTokenCompromized;

        if (IsExpired) return AuthErrors.Unauthorized("Session expired. Please log in again.");

        IsUsed = true;

        return Result.Success();
    }
}