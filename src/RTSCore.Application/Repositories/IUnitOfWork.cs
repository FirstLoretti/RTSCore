namespace RTSCore.Application.Repositories;

public interface IUnitOfWork
{
    IFactionRepository FactionRepository { get; }
    ICityRepository CityRepository { get; }
    IDiplomacyRelationRepository DiplomacyRelationRepository { get; }
    IDiplomacyOfferRepository DiplomacyOfferRepository { get; }
    IUserRepository UserRepository { get; }
    IRefreshTokenRepository RefreshTokenRepository { get; }
    IArmyRepository ArmyRepository { get; }

    Task SaveChangesAsync(CancellationToken ct);
}