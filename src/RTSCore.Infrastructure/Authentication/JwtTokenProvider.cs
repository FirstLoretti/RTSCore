using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using RTSCore.Application;
using RTSCore.Application.Authentication;
using RTSCore.Application.Common;
using RTSCore.Domain.Common;

namespace RTSCore.Infrastructure.Authentication;

public sealed class JwtTokenProvider(IOptions<JwtSettings> jwtSettings) : IJwtTokenProvider
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;

    public string Generate(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.Value.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Name),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(descriptor);

        return tokenHandler.WriteToken(token);
    }

    public Result<UserId> ValidateExpiredToken(string token)
    {
        var result = GetClaimsFromExpiredToken(token);
        if (result.IsFailure) return result.Error;

        var userIdClaim = result.Value.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        return !Guid.TryParse(userIdClaim, out var userId)
            ? AuthErrors.Unauthorized("Invalid token payload.")
            : new UserId(userId);
    }

    private Result<ClaimsPrincipal> GetClaimsFromExpiredToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        tokenHandler.InboundClaimTypeMap.Clear();

        var validationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = false,

            ValidIssuer = _jwtSettings.Issuer,
            ValidAudience = _jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret))
        };

        SecurityToken securityToken;
        ClaimsPrincipal claims;

        try
        {
            claims = tokenHandler.ValidateToken(token, validationParameters, out securityToken);
        }
        catch (Exception ex) when (ex is SecurityTokenException or ArgumentException)
        {
            return AuthErrors.Unauthorized("Invalid token.");
        }

        return securityToken is not JwtSecurityToken jwtToken
            || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase)
                ? AuthErrors.Unauthorized("Invalid token.")
                : claims;
    }
}