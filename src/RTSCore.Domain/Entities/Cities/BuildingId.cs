namespace RTSCore.Domain.Entities.Cities;

public readonly record struct BuildingId(Guid Value)
{
    public static BuildingId New() => new(Guid.NewGuid());
    public static BuildingId Empty => new(Guid.Empty);
}