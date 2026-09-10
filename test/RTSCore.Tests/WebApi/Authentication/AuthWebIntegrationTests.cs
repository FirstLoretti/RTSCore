using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using RTSCore.Application.Authentication.Commands;
using RTSCore.Application.Authentication.Common;
using RTSCore.Domain.Entities;
using RTSCore.Infrastructure.Persistence;
using RTSCore.Tests.Base;

namespace RTSCore.Tests.WebApi.Authentication;

public class AuthWebIntegrationTests(WebApplicationFactory<Program> factory) : WebTestBase(factory)
{
    [Fact]
    public async Task Register_WithValidCommand_ShouldReturnOk_WithValidJwtToken()
    {
        var command = new RegisterUserCommand("TestUser", "TestPassword");

        var response = await _client.PostAsJsonAsync("api/auth/register", command);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();

        Assert.NotNull(authResponse);
        Assert.False(string.IsNullOrWhiteSpace(authResponse.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(authResponse.RefreshToken));
    }

    [Fact]
    public async Task Login_WithValidCommand_ShouldReturnOk_WithValidJwtToken()
    {
        var name = "TestName";
        var password = "TestPassword";

        using (var scope = _factory.Services.CreateScope())
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            var user = new User(name, passwordHash);

            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            context.Users.Add(user);
            await context.SaveChangesAsync();
        }

        var command = new LoginUserCommand(name, password);
        var response = await _client.PostAsJsonAsync("api/auth/login", command);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();

        Assert.NotNull(authResponse);
        Assert.False(string.IsNullOrWhiteSpace(authResponse.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(authResponse.RefreshToken));
    }

    [Fact]
    public async Task Logout_WithValidJwtToken_ShouldReturnNoContent()
    {
        var user = new User("name", BCrypt.Net.BCrypt.HashPassword("password"));

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var refreshTokenEntity = new RefreshToken(user.Id, "token", DateTime.UtcNow.AddDays(30));

            context.Users.Add(user);
            context.RefreshTokens.Add(refreshTokenEntity);
            await context.SaveChangesAsync();
        }

        var loginCommand = new LoginUserCommand(user.Name, "password");
        var loginResponse = await _client.PostAsJsonAsync("api/auth/login", loginCommand);

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var authResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(authResult);

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, "api/auth/logout");
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authResult.AccessToken);

        var response = await _client.SendAsync(requestMessage);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Logout_WithInvalidJwtToken_ShouldReturnUnauthorized()
    {
        var token = "qwerSasddsccdsacd.P1a2222ksaaaaaZ.11112spAzzzQ";
        var requestMessange = new HttpRequestMessage(HttpMethod.Post, "api/auth/logout");
        requestMessange.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(requestMessange);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Logout_WithoutJwtToken_ShouldReturnUnauthorized()
    {
        var response = await _client.PostAsync("api/auth/logout", null);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}