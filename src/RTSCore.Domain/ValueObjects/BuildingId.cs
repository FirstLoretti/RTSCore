namespace RTSCore.Domain.ValueObjects;

public readonly record struct BuildingId
{
    public readonly string Value => field ?? "empty_id";

    public BuildingId(string value)
    {
        Value = !string.IsNullOrWhiteSpace(value)
            ? value.Trim().ToLowerInvariant()
            : throw new ArgumentException(
                "BuildingId не может быть пустым или состоять из пробелов", nameof(value));
    }

    public static implicit operator BuildingId(string value) => new(value);
    public static implicit operator string(BuildingId id) => id.Value;
}