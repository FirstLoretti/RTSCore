namespace RTSCore.Domain.Interfaces.Authentication;

public interface IRefreshTokenGenerator
{
    string Generate();
}