using System.Net;
using System.Net.Http.Json;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using RTSCore.Application.Campaign.Services;
using RTSCore.Domain.Entities;
using RTSCore.Domain.ValueObjects;
using RTSCore.Infrastructure.Persistence;
using RTSCore.Tests.Base;

namespace RTSCore.Tests.IntegrationTests.WebApi;

public class BattleControllerTests(WebApplicationFactory<Program> factory) : WebTestBase(factory)
{
    [Fact]
    public async Task Result_WithValidRequest_SholdReturnOkWithBattleResult()
    {
        var knightTemplate =
            new UnitTemplate(UnitType.Knight, "Knight", 1, 200, 20, 20, 1, 1, 1, 1, 1, UnitCategory.Infantry, 1);
        var attacker = Army.Create(
            FactionType.England, new(0f, 0f), 1, 2,
            Unit.CreateWithCustomStatus("id_1", FactionType.England, knightTemplate, 0));
        var defender = Army.Create(
            FactionType.France, new(0f, 0f), 1, 2,
            Unit.CreateWithCustomStatus("id_2", FactionType.France, knightTemplate, 0));

        attacker.AssignUnit(Unit.CreateWithCustomStatus("id_3", FactionType.England, knightTemplate, 0));

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            context.Armies.Add(attacker);
            context.Armies.Add(defender);
            await context.SaveChangesAsync();
        }

        var command = new AutoBattleCommand(attacker.Id, defender.Id);
        var response = await _client.PostAsJsonAsync("api/battle/calculate", command);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<BattleResult>();

        Assert.NotNull(result);
        Assert.True(result.IsAttackerWon);
        Assert.Equal(2, result.AttackerUnitsLogs.Count);
        Assert.Single(result.DefenderUnitsLogs);
    }
}