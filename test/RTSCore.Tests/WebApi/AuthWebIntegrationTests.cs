using System.Net;
using System.Net.Http.Json;

using Microsoft.AspNetCore.Mvc.Testing;

using RTSCore.Application.Authentication.Commands;
using RTSCore.Domain.ValueObjects;
using RTSCore.Tests.Base;

namespace RTSCore.Tests.WebApi;

public class AuthWebIntegrationTests(WebApplicationFactory<Program> factory): WebTestBase(factory)
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
}