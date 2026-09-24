namespace RTSCore.Domain.ValueObjects;

public readonly record struct ArmyId
{
    private readonly string _value;
    public string Value => _value
        ?? throw new ArgumentException("ArmyId не может быть пустым", nameof(_value));

    public ArmyId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("ArmyId не может быть пустым", nameof(value));

        _value = value.Trim().ToLowerInvariant();
    }

    public static implicit operator ArmyId(string value) => new(value);
    public static implicit operator string(ArmyId id) => id.Value;

    public override string ToString() => Value;
}