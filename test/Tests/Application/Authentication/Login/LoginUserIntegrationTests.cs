using FluentAssertions;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using RTSCore.Application;
using RTSCore.Application.Authentication;
using RTSCore.Application.Authentication.Login;
using RTSCore.Infrastructure.Persistence;

namespace Tests.Application.Authentication.Login;

[Collection("TestCollection")]
public sealed class LoginUserIntegrationTests(TestFixture testFixture) : IAsyncLifetime
{
    [Fact]
    public async Task Handle_ShouldReturnTokensAndRemoveThenSaveRefreshToken()
    {
        var user = new User("testName", "testPass");
        var token = TestData.CreateRefreshToken(userId: user.Id);

        using (var scope = testFixture.ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            context.Users.Add(user);
            context.RefreshTokens.Add(token);

            await context.SaveChangesAsync();
        }

        string tokenId;
        using (var scope = testFixture.ServiceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var result = await mediator.Send(new LoginUserCommand(user.Name, "testPass"));

            result.IsSuccess.Should().BeTrue();
            tokenId = result.Value.RefreshToken;
            result.Value.RefreshToken.Should().NotBe(token.Token);
            result.Value.AccessToken.Should().NotBeNullOrEmpty();
        }

        using (var scope = testFixture.ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var tokens = await context.RefreshTokens
                .AsNoTracking()
                .Where(t => t.UserId == user.Id)
                .ToListAsync();

            tokens.Should().ContainSingle().Which.Token.Should().Be(tokenId);
        }
    }

    public Task DisposeAsync() => Task.CompletedTask;

    public Task InitializeAsync() => testFixture.ResetDatabaseAsync();
}