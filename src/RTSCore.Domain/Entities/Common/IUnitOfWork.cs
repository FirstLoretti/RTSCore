using RTSCore.Domain.Entities.Campaign;
using RTSCore.Domain.Entities.Campaign.Diplomacy;
using RTSCore.Domain.Entities.Identity;

namespace RTSCore.Domain.Entities.Common;

public interface IUnitOfWork
{
    IBuildingRepository BuildingRepository { get; }
    IUnitRepository UnitRepository { get; }
    IFactionRepository FactionRepository { get; }
    ICityRepository CityRepository { get; }
    IDiplomacyRelationRepository DiplomacyRelationRepository { get; }
    IDiplomacyOfferRepository DiplomacyOfferRepository { get; }
    IUserRepository UserRepository { get; }
    IRefreshTokenRepository RefreshTokenRepository { get; }
    IArmyRepository ArmyRepository { get; }

    Task SaveChangesAsync(CancellationToken cancellationToken);
}