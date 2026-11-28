using FluentAssertions;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using RTSCore.Application.Authentication.Register;
using RTSCore.Infrastructure.Persistence;

namespace Tests.Application.Authentication.Register;

[Collection("TestCollection")]
public sealed class RegisterUserHandlerIntegrationTests(TestFixture testFixture) : IAsyncLifetime
{
    [Fact]
    public async Task Handle_ShouldRegisterUserAndSaveToDb()
    {
        using (var scope = testFixture.ServiceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var result = await mediator.Send(new RegisterUserCommand("name", "password"));

            result.IsSuccess.Should().BeTrue();
        }

        using (var scope = testFixture.ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var user = await context.Users
                .AsNoTracking()
                .FirstAsync(u => u.Name == "name");
            BCrypt.Net.BCrypt.Verify("password", user.PasswordHash).Should().BeTrue();

            var token = await context.RefreshTokens
                .AsNoTracking()
                .FirstAsync(t => t.UserId == user.Id);
        }
    }

    public Task DisposeAsync() => Task.CompletedTask;

    public Task InitializeAsync() => testFixture.ResetDatabaseAsync();
}