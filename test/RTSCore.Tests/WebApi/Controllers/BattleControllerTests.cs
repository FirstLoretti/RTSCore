using System.Net;
using System.Net.Http.Json;
using System.Numerics;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using RTSCore.Application.Campaign.AutoBattle;
using RTSCore.Domain.Entities;
using RTSCore.Domain.ValueObjects;
using RTSCore.Infrastructure.Persistence;
using RTSCore.Tests.Base;

namespace RTSCore.Tests.WebApi.Controllers;

public class BattleControllerTests(WebApplicationFactory<Program> factory) : WebTestBase(factory)
{
    [Fact]
    public async Task Result_WithValidRequest_SholdReturnOkWithBattleResult()
    {
        var attacker = Army.Create(FactionType.England, Vector2.Zero, new UnitTemplate());
        var defender = Army.Create(FactionType.France, Vector2.Zero, new UnitTemplate());

        attacker.RecruitUnit(new UnitTemplate());

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
        Assert.Equal(2, result.AttackerBattleLogs.Count);
        Assert.Single(result.DefenderBattleLogs);
    }
}