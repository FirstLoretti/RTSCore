using FluentAssertions;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using RTSCore.Application.Authentication.Logout;
using RTSCore.Infrastructure.Persistence;

namespace Tests.Application.Authentication.Logout;

[Collection("TestCollection")]
public sealed class LogoutUserIntegrationTests(TestFixture testFixture) : IAsyncLifetime
{
    [Fact]
    public async Task Handle_ShouldRemoveRefreshTokens()
    {
        var user = TestData.CreateUser();
        var token1 = TestData.CreateRefreshToken(userId: user.Id);
        var token2 = TestData.CreateRefreshToken(userId: user.Id, token: "token2");

        using (var scope = testFixture.ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            context.Users.Add(user);
            context.RefreshTokens.Add(token1);
            context.RefreshTokens.Add(token2);

            await context.SaveChangesAsync();
        }

        using (var scope = testFixture.ServiceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            await mediator.Send(new LogoutUserCommand(token1.UserId));
        }

        using (var scope = testFixture.ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var haveTokens = await context.RefreshTokens.AnyAsync();
            haveTokens.Should().BeFalse();
        }
    }

    public Task DisposeAsync() => Task.CompletedTask;

    public Task InitializeAsync() => testFixture.ResetDatabaseAsync();
}