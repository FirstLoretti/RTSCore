using FluentAssertions;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using RTSCore.Application;
using RTSCore.Application.Authentication;
using RTSCore.Application.Authentication.Refresh;
using RTSCore.Infrastructure.Persistence;

namespace Tests.Application.Authentication.Refresh;

[Collection("TestCollection")]
public sealed class RefreshTokenHandlerIntegrationTests(TestFixture testFixture) : IAsyncLifetime
{
    [Fact]
    public async Task Handle_ShouldRefreshTokens()
    {
        string accessToken;
        string refreshToken;
        RefreshToken refreshTokenEntity;

        using (var scope = testFixture.ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var jwtTokenProvider = scope.ServiceProvider.GetRequiredService<IJwtTokenProvider>();
            var refreshTokenGenerator = scope.ServiceProvider.GetRequiredService<IRefreshTokenGenerator>();

            var user = new User("name", "pass");
            refreshToken = refreshTokenGenerator.Generate();
            refreshTokenEntity = TestData.CreateRefreshToken(userId: user.Id, token: refreshToken);
            accessToken = jwtTokenProvider.Generate(user);

            context.Users.Add(user);
            context.RefreshTokens.Add(refreshTokenEntity);
            await context.SaveChangesAsync();
        }

        string createdRefreshToken;
        using (var scope = testFixture.ServiceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var result = await mediator.Send(new RefreshTokensCommand(accessToken, refreshToken));

            result.IsSuccess.Should().BeTrue();
            createdRefreshToken = result.Value.RefreshToken;
            result.Value.AccessToken.Should().NotBe(accessToken);
            result.Value.RefreshToken.Should().NotBe(refreshToken);
        }

        using (var scope = testFixture.ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var dbRefreshTokens = await context.RefreshTokens
                .AsNoTracking()
                .Where(t => t.UserId == refreshTokenEntity.UserId)
                .ToListAsync();

            dbRefreshTokens.Should().HaveCount(2);
            dbRefreshTokens.Should().ContainSingle(t => t.IsUsed);
            dbRefreshTokens.Should().ContainSingle(t => t.Token == createdRefreshToken && !t.IsUsed);
        }
    }

    public Task DisposeAsync() => Task.CompletedTask;

    public Task InitializeAsync() => testFixture.ResetDatabaseAsync();
}