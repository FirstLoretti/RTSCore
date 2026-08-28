namespace RTSCore.Domain.Interfaces;

public interface IUnitOfWork
{
    IBuildingRepository BuildingRepository { get; }
    IUnitRepository UnitRepository { get; }
    IFactionRepository FactionRepository { get; }
    ICityRepository CityRepository { get; }

    Task SaveChangesAsync(CancellationToken cancellationToken);
}