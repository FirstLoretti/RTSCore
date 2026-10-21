using System.Diagnostics.CodeAnalysis;

using RTSCore.Domain.Errors;

namespace RTSCore.Domain.Common;

public readonly record struct Result<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public GameError? Error { get; }

    [AllowNull]
    public T Value => IsSuccess
        ? field!
        : throw new InvalidOperationException("Cannot access Value on a failed result. Check IsSuccess first.");

    private Result(bool isSuccess, T? result, GameError? error)
    {
        IsSuccess = isSuccess;
        Value = result;
        Error = error;
    }

    public static Result<T> Success(T value) => new(true, value, null);
    public static Result<T> Failure(GameError error) => new(false, default, error);

    public static implicit operator Result<T>(T value) => Success(value);
    public static implicit operator Result<T>(GameError error) => Failure(error);
}