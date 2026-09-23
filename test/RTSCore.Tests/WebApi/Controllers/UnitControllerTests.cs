using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

using RTSCore.Domain.Entities;
using RTSCore.Domain.ValueObjects;
using RTSCore.Infrastructure.Persistence;
using RTSCore.Tests.Base;
using RTSCore.Application.Campaign.DisbandUnit;
using System.Numerics;

namespace RTSCore.Tests.WebApi.Controllers;

public class UnitControllerTests(WebApplicationFactory<Program> program) : WebTestBase(program)
{
    [Fact]
    public async Task DisbandUnit_WithValidCommand_ShoulReturnNoContent()
    {
        var army = Army.Create(FactionType.England, Vector2.Zero, new UnitTemplate());
        army.RecruitUnit(new UnitTemplate() { TurnsToRecruit = 0 });

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            context.Armies.Add(army);
            await context.SaveChangesAsync();
        }

        var response = await _client.DeleteAsync($"api/unit/{army.Units[1].Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Disband_ShouldReturn422_WhenUnitInvulnerable()
    {
        var army = Army.Create(FactionType.England, Vector2.Zero, new UnitTemplate());
        army.RecruitUnit(new UnitTemplate() { Type = UnitType.Invulnerable, TurnsToRecruit = 0 });

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            context.Add(army);
            await context.SaveChangesAsync();
        }

        var response = await _client.DeleteAsync($"api/unit/{army.Units[1].Id}");

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);

        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.NotNull(problemDetails);
        Assert.Equal("Нарушение игровых правил", problemDetails.Title);
        Assert.Equal(
            $"[{nameof(DisbandUnitCommand)}] " +
            $"Юнита {army.Units[1]} с типом {UnitType.Invulnerable} нельзя удалить из базы данных",
            problemDetails.Detail
        );
    }
}