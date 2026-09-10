using System.Net;
using System.Net.Http.Json;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using RTSCore.Application.Authentication.Commands;
using RTSCore.Application.Authentication.Common;
using RTSCore.Domain.Entities;
using RTSCore.Domain.Interfaces.Authentication;
using RTSCore.Infrastructure.Persistence;
using RTSCore.Tests.Base;

namespace RTSCore.Tests.WebApi.Authentication;

public class RefreshTokensEndpointTests(WebApplicationFactory<Program> program) : WebTestBase(program)
{
    [Fact]
    public async Task Refresh_WithValidTokens_ShouldRotateTokensAndReturnOk()
    {
        string accessToken;
        string refreshToken;
        using (var scope = _factory.Services.CreateScope())
        {
            var user = new User("name", BCrypt.Net.BCrypt.HashPassword("password"));
            var jwtTokenGenerator = scope.ServiceProvider.GetRequiredService<IJwtTokenGenerator>();
            var refreshTokenGenerator = scope.ServiceProvider.GetRequiredService<IRefreshTokenGenerator>();

            accessToken = jwtTokenGenerator.Generate(user);
            refreshToken = refreshTokenGenerator.Generate();
            var refreshTokenEntity = new RefreshToken(user.Id, refreshToken, DateTime.UtcNow.AddDays(30));

            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            context.Users.Add(user);
            context.RefreshTokens.Add(refreshTokenEntity);
            await context.SaveChangesAsync();
        }

        var command = new RefreshTokensCommand(accessToken, refreshToken);
        var response = await _client.PostAsJsonAsync("api/auth/refresh", command);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();

        Assert.NotNull(authResponse);
        Assert.NotEqual(accessToken, authResponse.AccessToken);
        Assert.NotEqual(refreshToken, authResponse.RefreshToken);
    }
}