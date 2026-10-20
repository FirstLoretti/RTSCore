namespace RTSCore.Domain.Entities.Diplomacy;

public readonly record struct DiplomacyRelationId(Guid Value)
{
    public static DiplomacyRelationId New() => new(Guid.NewGuid());
    public static DiplomacyRelationId Empty => new(Guid.Empty);
}