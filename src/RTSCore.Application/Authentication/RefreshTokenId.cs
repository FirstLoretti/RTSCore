namespace RTSCore.Application.Authentication;

public readonly record struct RefreshTokenId(Guid Value)
{
    public static RefreshTokenId New() => new(Guid.NewGuid());
    public static RefreshTokenId Empty => default;
}