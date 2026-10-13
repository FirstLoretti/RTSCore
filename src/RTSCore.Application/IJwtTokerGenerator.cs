using System.Security.Claims;

using RTSCore.Domain.Entities;

namespace RTSCore.Domain.Common;

public interface IJwtTokenGenerator
{
    string Generate(User user);

    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}