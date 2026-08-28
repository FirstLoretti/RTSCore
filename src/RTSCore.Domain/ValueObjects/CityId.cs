namespace RTSCore.Domain.ValueObjects;

public readonly record struct CityId
{
    public string Value => field ?? "empty_id";

    public CityId(string value)
    {
        Value = !string.IsNullOrWhiteSpace(value)
            ? value.Trim().ToLowerInvariant()
            : throw new ArgumentException(
                "CityId не может быть пустым или состоять из пробелов", nameof(value)
            );
    }

    public static implicit operator CityId(string value) => new(value);
    public static implicit operator string(CityId id) => id.Value;

    public override string ToString() => Value;
}