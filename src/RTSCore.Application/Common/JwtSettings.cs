namespace RTSCore.Application.Common;

public sealed class JwtSettings
{
    public const string SectionName = "JwtSettings";

    public required string Secret { get; init; }
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public int ExpiryInMinutes { get; init; }
}