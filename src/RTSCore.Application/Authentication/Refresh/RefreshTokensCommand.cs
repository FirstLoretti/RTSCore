using MediatR;

using RTSCore.Domain.Common;

namespace RTSCore.Application.Authentication.Refresh;

public sealed record RefreshTokensCommand(string AccessToken, string RefreshToken) : IRequest<Result<AuthResponse>>;