namespace RTSCore.Application.Authentication;

public sealed record AuthResponse(string AccessToken, string RefreshToken);