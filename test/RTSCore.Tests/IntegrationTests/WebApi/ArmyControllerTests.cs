using System.Net;
using System.Net.Http.Json;
using System.Numerics;
using System.Text.Json;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using RTSCore.Application.Army.Commands;
using RTSCore.Domain.Entities;
using RTSCore.Domain.ValueObjects;
using RTSCore.Infrastructure.Persistence;
using RTSCore.Tests.Base;

namespace RTSCore.Tests.IntegrationTests.WebApi;

public class ArmyControllerTests(WebApplicationFactory<Program> factory) : WebTestBase(factory)
{
    [Fact]
    public async Task Move_WithValidRequest_ShouldReturnOkWithArmyCommandResponse()
    {
        var destination = new Vector2(10f, 10f);
        var unitTemplate = new UnitTemplate(UnitType.Knight, "Unit", 1, 1, 1, 1, 1, 1, 1, 1, 1, UnitCategory.Infantry, 1);
        var general = Unit.CreateWithCustomStatus("id", FactionType.England, unitTemplate, 0);
        var army = Army.Create(FactionType.England, new(0f, 0f), 1000, 20, general);
        var command = new MoveArmyCommand(army.Id, destination.X, destination.Y);

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            context.Units.Add(general);
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