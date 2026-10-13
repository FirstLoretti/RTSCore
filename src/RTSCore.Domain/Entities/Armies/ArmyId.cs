namespace RTSCore.Domain.ValueObjects.Identifiers;

public readonly record struct ArmyId(Guid Value)
{
    public static ArmyId New() => new(Guid.NewGuid());
    public static ArmyId Empty => new(Guid.Empty);
}