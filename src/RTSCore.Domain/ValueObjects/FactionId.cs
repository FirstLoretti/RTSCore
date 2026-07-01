namespace RTSCore.Domain.ValueObjects;

public readonly record struct FactionId
{
    public readonly string Value => field ?? "empty_id";

    public FactionId(string value)
    {
        Value = !string.IsNullOrWhiteSpace(value)
            ? value.Trim().ToLowerInvariant()
            : throw new ArgumentException(
                "FactionId не может быть пустым или состоять из пробелов", nameof(value)
            );
    }

    public static implicit operator FactionId(string value) => new(value);
}