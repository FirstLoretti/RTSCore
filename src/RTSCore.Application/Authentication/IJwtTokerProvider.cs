using RTSCore.Domain.Common;

namespace RTSCore.Application.Authentication;

public interface IJwtTokenProvider
{
    string Generate(User user);

    Result<UserId> ValidateExpiredToken(string token);
}