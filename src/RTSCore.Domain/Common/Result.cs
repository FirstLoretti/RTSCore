using System.Diagnostics.CodeAnalysis;

using RTSCore.Domain.Errors;

namespace RTSCore.Domain.Common;

public readonly record struct Result
{
    public bool IsSuccess { get; }

    [MemberNotNullWhen(true, nameof(Error))]
    public bool IsFailure => !IsSuccess;
    public GameError? Error { get; }

    private Result(bool isSuccess, GameError? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, null);
    public static Result Failure(GameError error) => new(false, error);

    public static implicit operator Result(GameError error) => Failure(error);
}