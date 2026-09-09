using System.IdentityModel.Tokens.Jwt;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using RTSCore.Application.Authentication.Commands;
using RTSCore.Application.Authentication.Common;
using RTSCore.Domain.Entities;
using RTSCore.Domain.Exeptions;
using RTSCore.Infrastructure.Persistence;
using RTSCore.Tests.Base;

namespace RTSCore.Tests.Application.Authentication;

public class RegisterUserCommandHandlerTests : TestBase
{
    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public async Task Handle_RegistrationScenarios_ShouldBehaveCorrectly(
        bool alreadyExist,
        bool shouldSucceed
    )
    {
        var serviceProvider = SetupTestInvironment();

        var name = "TestName";
        var password = "TestPassword";

        if (alreadyExist)
        {
            using var scope = serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var existingUser = new User("TestName", "TestPassword");

            context.Users.Add(existingUser);
            await context.SaveChangesAsync();
        }

        AuthResponse authResponse;
        using (var scope = serviceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var command = new RegisterUserCommand(name, password);

            if (shouldSucceed)
            {
                authResponse = await mediator.Send(command);
            }
            else
            {
                await Assert.ThrowsAsync<GameRuleException>(async () => await mediator.Send(command));
                return;
            }
        }

        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var dbUser = await context.Users.FirstOrDefaultAsync(u => u.Name == name);
            var dbRefreshToken = await context.RefreshTokens.SingleOrDefaultAsync();

            Assert.NotNull(dbRefreshToken);
            Assert.NotNull(dbUser);
            Assert.True(BCrypt.Net.BCrypt.Verify(password, dbUser.PasswordHash));

            Assert.False(string.IsNullOrWhiteSpace(authResponse.AccessToken));
            Assert.False(string.IsNullOrWhiteSpace(authResponse.RefreshToken));

            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(authResponse.AccessToken);
            var factionClaim = jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.UniqueName).Value;
            Assert.Equal(name, factionClaim);
        }
    }
}