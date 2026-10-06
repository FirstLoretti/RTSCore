using System.Security.Claims;

namespace RTSCore.Domain.Entities.Identity;

public interface IJwtTokenGenerator
{
    string Generate(User user);

    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}