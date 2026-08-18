namespace RTSCore.Domain.Interfaces;

public interface IUnitOfWork
{
    IBuildingRepository BuildingRepository { get; }
    IUnitRepository UnitRepository { get; }
    Task SaveChangesAsync(CancellationToken cancellationToken);
}