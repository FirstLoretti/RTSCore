namespace RTSCore.Domain.ValueObjects.Identifiers;

public readonly record struct UnitId(Guid Value)
{
    public static UnitId New() => new(Guid.NewGuid());
    public static UnitId Empty() => new(Guid.Empty);
}