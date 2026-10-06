namespace RTSCore.Domain.Entities.Identity;

public interface IRefreshTokenGenerator
{
    string Generate();
}