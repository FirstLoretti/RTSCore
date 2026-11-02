namespace RTSCore.Application.Authentication;

public interface IRefreshTokenGenerator
{
    string Generate();
}