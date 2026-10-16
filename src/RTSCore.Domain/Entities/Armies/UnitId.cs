namespace RTSCore.Domain.Entities.Armies;

public readonly record struct UnitId(Guid Value)
{
    public static UnitId New() => new(Guid.NewGuid());
    public static UnitId Empty => default;
}