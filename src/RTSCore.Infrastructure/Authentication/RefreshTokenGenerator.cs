using System.Security.Cryptography;

using RTSCore.Application.Authentication;

namespace RTSCore.Infrastructure.Authentication;

public sealed class RefreshTokenGenerator : IRefreshTokenGenerator
{
    public string Generate()
    {
        var bytes = new byte[64];
        var rng = RandomNumberGenerator.Create();

        rng.GetBytes(bytes);

        return Convert.ToBase64String(bytes);
    }
}