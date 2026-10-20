using RTSCore.Domain.Errors;

namespace RTSCore.Domain.Common;

public readonly record struct Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public GameError? Error { get; }

    private Result(bool isSuccess, GameError? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, null);
    public static Result Failure(GameError error) => new(false, error);
}