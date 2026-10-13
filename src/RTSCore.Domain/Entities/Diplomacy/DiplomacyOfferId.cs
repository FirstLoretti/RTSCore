namespace RTSCore.Domain.ValueObjects.Identifiers;

public readonly record struct DiplomacyOfferId(Guid Value)
{
    public static DiplomacyOfferId New() => new(Guid.NewGuid());
    public static DiplomacyOfferId Empty() => new(Guid.Empty);
}