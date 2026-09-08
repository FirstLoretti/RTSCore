using System.Net;
using System.Net.Http.Json;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using RTSCore.Application.Authentication.Commands;
using RTSCore.Domain.Entities;
using RTSCore.Domain.ValueObjects;
using RTSCore.Infrastructure.Persistence;
using RTSCore.Tests.Base;

namespace RTSCore.Tests.WebApi;

public class AuthWebIntegrationTests(WebApplicationFactory<Program> factory) : WebTestBase(factory)
{
    [Fact]
    public async Task Register_WithValidCommand_ShouldReturnOk_WithValidJwtToken()
    {
        var command = new RegisterUserCommand("TestUser", "TestPassword", FactionType.England);

        var response = await _client.PostAsJsonAsync("api/auth/register", command);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var token = await response.Content.ReadAsStringAsync();

        Assert.False(string.IsNullOrWhiteSpace(token));
    }

    [Fact]
    public async Task Login_WithValidCommand_ShouldReturnOk_WithValidJwtToken()
    {
        var name = "TestName";
        var password = "TestPassword";

        using (var scope = _factory.Services.CreateScope())
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            var user = new User(name, passwordHash, FactionType.England);

            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            context.Users.Add(user);
            await context.SaveChangesAsync();
        }

        var command = new LoginUserCommand(name, password);
        var response = await _client.PostAsJsonAsync("api/auth/login", command);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var token = await response.Content.ReadAsStringAsync();
        Assert.False(string.IsNullOrWhiteSpace(token));
    }
}