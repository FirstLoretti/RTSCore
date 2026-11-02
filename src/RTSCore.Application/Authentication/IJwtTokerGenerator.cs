using System.Security.Claims;

namespace RTSCore.Application.Authentication;

public interface IJwtTokenGenerator
{
    string Generate(User user);

    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}