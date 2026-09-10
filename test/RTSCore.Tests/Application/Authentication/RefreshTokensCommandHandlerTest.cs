using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using RTSCore.Application.Authentication.Commands;
using RTSCore.Application.Authentication.Common;
using RTSCore.Domain.Entities;
using RTSCore.Domain.Exeptions;
using RTSCore.Domain.Interfaces.Authentication;
using RTSCore.Infrastructure.Persistence;
using RTSCore.Tests.Base;

namespace RTSCore.Tests.Application.Authentication;

public class RefreshTokensCommanHandlerTest : TestBase
{
    [Fact]
    public async Task Handle_WithValidTokens_ShouldRotateTokensAndRetrunAuthResponse()
    {
        var serviceProvider = SetupTestInvironment();

        var (user, accessToken, refreshToken) = await SeedDataBaseAsync(
            serviceProvider,
            isRefreshTokenExpired: false,
            isRefreshTokenUsed: false
        );

        var authResponse = await SendRefreshCommandAsync(serviceProvider, accessToken, refreshToken);

        Assert.NotNull(authResponse);
        Assert.NotEqual(refreshToken, authResponse.RefreshToken);
        Assert.NotEqual(accessToken, authResponse.AccessToken);

        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var oldToken = await context.RefreshTokens.FirstAsync(t => t.Token == refreshToken);
        Assert.True(oldToken.IsUsed);

        var hasNewToken = await context.RefreshTokens.AnyAsync(t => t.Token == authResponse.RefreshToken && t.UserId == user.Id);
        Assert.True(hasNewToken);
    }

    [Theory]
    [InlineData(true, false, "Session expired. Please log in again.")]
    [InlineData(false, true, "Security breach detected. Please log in again.")]
    public async Task Handle_WhenRefreshTokenInvalid_ShouldThrow(
        bool isTokenExpiredInDb,
        bool isTokenUsed,
        string exceptionMessage
    )
    {
        var serviceProvider = SetupTestInvironment();

        var (user, accessToken, refreshToken) = await SeedDataBaseAsync(serviceProvider, isTokenExpiredInDb, isTokenUsed);

        var exception = await Assert.ThrowsAsync<GameRuleException>(
            async () => await SendRefreshCommandAsync(serviceProvider, accessToken, refreshToken)
        );

        Assert.Equal(exceptionMessage, exception.Message);

        if (isTokenUsed)
        {
            using var scope = serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var hasTokens = await context.RefreshTokens.AnyAsync();
            Assert.False(hasTokens);
        }
    }

    [Theory]
    [InlineData("Invalid access token.")]
    public async Task Handle_WithInvalidAcessToken_ShouldThrow(string exceptionMessage)
    {
        var serviceProvider = SetupTestInvironment();

        var (user, accessToken, refreshToken) = await SeedDataBaseAsync(
            serviceProvider,
            isRefreshTokenExpired: false,
            isRefreshTokenUsed: false
        );

        var invalidAccessToken = accessToken + "1";
        var exception = await Assert.ThrowsAsync<GameRuleException>(
            async () => await SendRefreshCommandAsync(serviceProvider, invalidAccessToken, refreshToken)
        );

        Assert.Equal(exceptionMessage, exception.Message);
    }

    private static async Task<(User user, string accessToken, string refreshToken)> SeedDataBaseAsync(
        IServiceProvider serviceProvider,
        bool isRefreshTokenExpired,
        bool isRefreshTokenUsed
    )
    {
        var user = new User("name", BCrypt.Net.BCrypt.HashPassword("password"));

        string accessToken;
        string refreshToken;

        using var scope = serviceProvider.CreateScope();

        var jwtTokenGenerator = scope.ServiceProvider.GetRequiredService<IJwtTokenGenerator>();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var refreshTokenGenerator = scope.ServiceProvider.GetRequiredService<IRefreshTokenGenerator>();

        accessToken = jwtTokenGenerator.Generate(user);
        refreshToken = refreshTokenGenerator.Generate();

        var refreshTokenEntity = isRefreshTokenExpired
            ? new RefreshToken(user.Id, refreshToken, DateTime.UtcNow.AddDays(-1))
            : new RefreshToken(user.Id, refreshToken, DateTime.UtcNow.AddDays(30));

        if (isRefreshTokenUsed) refreshTokenEntity.Use();

        context.RefreshTokens.Add(refreshTokenEntity);
        context.Users.Add(user);
        await context.SaveChangesAsync();

        return (user, accessToken, refreshToken);
    }

    private static async Task<AuthResponse> SendRefreshCommandAsync(
        IServiceProvider serviceProvider,
        string accessToken,
        string refreshToken
    )
    {
        using var scope = serviceProvider.CreateScope();

        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        return await mediator.Send(new RefreshTokensCommand(accessToken, refreshToken));
    }
}