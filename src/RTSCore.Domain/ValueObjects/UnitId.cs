namespace RTSCore.Domain.ValueObjects;

public readonly record struct UnitId
{
    public readonly string Value => field ?? "empty_id";

    public UnitId(string value)
    {
        Value = !string.IsNullOrWhiteSpace(value)
            ? value.Trim().ToLowerInvariant()
            : throw new ArgumentException(
                "UnitId не может быть пустым или состоять из пробелов", nameof(value));
    }

    public static implicit operator UnitId(string value) => new(value);
    public static implicit operator string(UnitId id) => id.Value;
}