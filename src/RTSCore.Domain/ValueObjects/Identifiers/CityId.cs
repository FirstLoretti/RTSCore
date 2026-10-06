namespace RTSCore.Domain.ValueObjects.Identifiers;

public readonly record struct CityId(Guid Value)
{
    public static CityId New() => new(Guid.NewGuid());
    public static CityId Empty() => new(Guid.Empty);
}