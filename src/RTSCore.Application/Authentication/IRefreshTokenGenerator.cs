using RTSCore.Domain.Common;

namespace RTSCore.Application.Authentication;

public interface IRefreshTokenGenerator
{
    string Generate();
}