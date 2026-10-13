namespace RTSCore.Domain.Common;

public interface IRefreshTokenGenerator
{
    string Generate();
}