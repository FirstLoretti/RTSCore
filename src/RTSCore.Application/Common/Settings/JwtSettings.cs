namespace RTSCore.Application.Common.Settings;

public class JwtSettings
{
    public const string SectionName = "JwSettings";

    public string Secret { get; init; } = string.Empty;
    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public int ExpiryInMinutes { get; init; }
}