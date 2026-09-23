using System.Net;
using System.Net.Http.Json;
using System.Numerics;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using RTSCore.Application.Campaign.ArmyMovement;
using RTSCore.Domain.Entities;
using RTSCore.Domain.ValueObjects;
using RTSCore.Infrastructure.Persistence;
using RTSCore.Tests.Base;

namespace RTSCore.Tests.WebApi.Controllers;

public class ArmyControllerTests(WebApplicationFactory<Program> factory) : WebTestBase(factory)
{
    [Fact]
    public async Task Move_WithValidRequest_ShouldReturnOkWithArmyCommandResponse()
    {
        var destination = new Vector2(1f, 1f);
        var army = Army.Create(FactionType.England, Vector2.Zero, new UnitTemplate());
        var command = new MoveArmyCommand(army.Id, destination.X, destination.Y);

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            context.Armies.Add(army);
            await context.SaveChangesAsync();
        }

        var response = await _client.PostAsJsonAsync("api/army/move", command);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<MoveArmyCommand>();

        Assert.NotNull(content);
        Assert.Equal(destination.X, content.X);
        Assert.Equal(destination.Y, content.Y);
        Assert.Equal(army.Id, content.ArmyId);
    }
}