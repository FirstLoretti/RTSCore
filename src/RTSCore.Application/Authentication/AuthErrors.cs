using RTSCore.Domain.Errors;

namespace RTSCore.Application.Authentication;

public static class AuthErrors
{
    public static Error InvalidCredentials { get; } = new(
        "Auth.InvalidCredentials",
        "Invalid username or password."
    );

    public static Error Unauthorized(string message) => new(
        "Auth.UnatorizedError",
        message
    );

    public static Error RefreshTokenCompromized { get; } = new(
        "Auth.RefreshTokenCompromized",
        "Refresh token is compromized. Please log in again."
    );
}