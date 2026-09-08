using System.IdentityModel.Tokens.Jwt;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using RTSCore.Application.Authentication.Commands;
using RTSCore.Domain.Entities;
using RTSCore.Domain.Exeptions;
using RTSCore.Domain.ValueObjects;
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
        var faction = FactionType.England;

        if (alreadyExist)
        {
            using var scope = serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var existingUser = new User("TestName", "TestPassword", FactionType.France);

            context.Users.Add(existingUser);
            await context.SaveChangesAsync();
        }

        string token = string.Empty;
        using (var scope = serviceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var command = new RegisterUserCommand(name, password, faction);

            if (shouldSucceed)
            {
                token = await mediator.Send(command);
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

            Assert.NotNull(dbUser);
            Assert.Equal(faction, dbUser.Faction);
            Assert.True(BCrypt.Net.BCrypt.Verify(password, dbUser.PasswordHash));

            Assert.False(string.IsNullOrWhiteSpace(token));
            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var factionClaim = jwtToken.Claims.First(c => c.Type == "faction").Value;
            Assert.Equal(faction.ToString(), factionClaim);
        }
    }
}