namespace RTSCore.Domain.Common;

public readonly record struct Error(string Id, string Message)
{
    public static readonly Error None = new(string.Empty, string.Empty);

    public static Error Combat(string id, string message)
        => new($"Combat.{id}", message);

    public static Error Economic(string id, string message)
        => new($"Economic.{id}", message);
}