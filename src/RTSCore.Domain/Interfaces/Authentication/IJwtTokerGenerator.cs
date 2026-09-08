using RTSCore.Domain.Entities;

namespace RTSCore.Domain.Interfaces.Authentication;

public interface IJwtTokenGenerator
{
    string Generate(User user);
}