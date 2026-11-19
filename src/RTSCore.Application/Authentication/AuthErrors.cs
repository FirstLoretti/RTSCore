using RTSCore.Domain.Errors;

namespace RTSCore.Application.Authentication;

public static class AuthErrors
{
    public static Error InvalidCredentials { get; } = new(
        "Auth.InvalidCredentials",
        "Invalid username or password."
    );
}