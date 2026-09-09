using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using RTSCore.Application.Authentication.Commands;
using RTSCore.Domain.Entities;
using RTSCore.Infrastructure.Persistence;
using RTSCore.Tests.Base;

namespace RTSCore.Tests.Application.Authentication;

public class LogoutUserCommandHandlerTests : TestBase
{
    [Fact]
    public async Task Handle_WhenUserLogsOut_ShouldRemoveAllRefreshUserTokensFormDb()
    {
        var serviceProvider = SetupTestInvironment();

        var user = new User("user", BCrypt.Net.BCrypt.HashPassword("password"));

        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var refreshTokenPc = new RefreshToken(user.Id, "pc", DateTime.UtcNow.AddDays(30));
            var refreshTokenPhone = new RefreshToken(user.Id, "phone", DateTime.UtcNow.AddDays(30));

            context.Users.Add(user);
            context.RefreshTokens.Add(refreshTokenPc);
            context.RefreshTokens.Add(refreshTokenPhone);
            await context.SaveChangesAsync();
        }

        using (var scope = serviceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var command = new LogoutUserCommand(user.Id);

            await mediator.Send(command);
        }

        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var tokensCount = await context.RefreshTokens.CountAsync(t => t.UserId == user.Id);

            Assert.Equal(0, tokensCount);
        }
    }
}