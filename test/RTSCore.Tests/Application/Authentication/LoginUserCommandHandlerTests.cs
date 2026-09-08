using System.IdentityModel.Tokens.Jwt;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

using RTSCore.Application.Authentication.Commands;
using RTSCore.Domain.Entities;
using RTSCore.Domain.Exeptions;
using RTSCore.Domain.ValueObjects;
using RTSCore.Infrastructure.Persistence;
using RTSCore.Tests.Base;

namespace RTSCore.Tests.Application.Authentication;

public class LoginUserCommandHandlerTests : TestBase
{
    [Theory]
    [InlineData("CorrectName", "CorrectPassword", true)]
    [InlineData("CorrectName", "WrongPassword", false)]
    [InlineData("WrongName", "CorrectPassword", false)]
    public async Task Handle_LoginScenarios_ShouldBehaveCorrectly(
        string loginName,
        string loginPassword,
        bool shouldSucceed
    )
    {
        var serviceProvider = SetupTestInvironment();
        var name = "CorrectName";
        var password = "CorrectPassword";
        var faction = FactionType.England;

        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            context.Add(new User(name, passwordHash, faction));
            await context.SaveChangesAsync();
        }

        string token = string.Empty;
        using (var scope = serviceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var command = new LoginUserCommand(loginName, loginPassword);

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

        Assert.False(string.IsNullOrWhiteSpace(token));

        var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
        var factionClaim = jwtToken.Claims.First(c => c.Type == "faction").Value;

        Assert.Equal(faction.ToString(), factionClaim);
    }
}