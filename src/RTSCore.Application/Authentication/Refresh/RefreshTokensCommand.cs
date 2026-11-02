using MediatR;

namespace RTSCore.Application.Authentication.Refresh;

public sealed record RefreshTokensCommand(string AccessToken, string RefreshToken) : IRequest<AuthResponse>;