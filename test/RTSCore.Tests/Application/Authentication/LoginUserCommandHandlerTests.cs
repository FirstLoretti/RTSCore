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

        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            context.Add(new User(name, passwordHash));
            await context.SaveChangesAsync();
        }

        AuthResponse authResponse;
        using (var scope = serviceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var command = new LoginUserCommand(loginName, loginPassword);

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
            var refreshToken = await context.RefreshTokens.SingleOrDefaultAsync();

            Assert.NotNull(refreshToken);
        }

        Assert.False(string.IsNullOrWhiteSpace(authResponse.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(authResponse.RefreshToken));

        var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(authResponse.AccessToken);
        var nameClaim = jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.UniqueName).Value;

        Assert.Equal(name, nameClaim);
    }
}