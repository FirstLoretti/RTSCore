namespace RTSCore.Domain.Common;

public readonly record struct Result<T>
{
    public readonly T? Value
        => IsSuccess
            ? field
            : throw new InvalidOperationException("Нельзя прочитать значение неуспешного результата");

    public readonly Error Error { get; init; }

    public readonly bool IsSuccess { get; init; }

    public Result(T? value, bool isSuccess, Error error)
    {
        Value = value;
        IsSuccess = isSuccess;
        Error = error;
    }

    public static implicit operator Result<T>(T value) => new(value, true, Error.None);

    public static implicit operator Result<T>(Error error) => new(default, false, error);
}