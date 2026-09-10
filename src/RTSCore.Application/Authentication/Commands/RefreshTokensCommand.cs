using MediatR;

using RTSCore.Application.Authentication.Common;

namespace RTSCore.Application.Authentication.Commands;

public record RefreshTokensCommand(string AccessToken, string RefreshToken) : IRequest<AuthResponse>;