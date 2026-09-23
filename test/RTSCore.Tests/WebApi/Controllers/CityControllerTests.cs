using System.Net;
using System.Net.Http.Json;
using System.Numerics;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using RTSCore.Application.Campaign.CityConstruction;
using RTSCore.Application.Campaign.Common;
using RTSCore.Application.Campaign.UnitRecruitment;
using RTSCore.Domain.Entities;
using RTSCore.Domain.Services;
using RTSCore.Domain.ValueObjects;
using RTSCore.Domain.ValueObjects.Presets;
using RTSCore.Infrastructure.Persistence;
using RTSCore.Tests.Base;

namespace RTSCore.Tests.WebApi.Controllers;

public class CityControllerTests(WebApplicationFactory<Program> program) : WebTestBase(program)
{
    [Fact]
    public async Task ConstructBuilding_WithValidCommand_ShouldReturnNoContent()
    {
        var cityId = new CityId("test_london");
        var barrackCost = GameBalance.Buildings.GetTemplate(BuildingType.ReqruitBarrack).Cost;

        using var scope = await SeedTestWorldAsync(cityId, barrackCost);

        var command = new ConstructBuildingCommand(cityId, BuildingType.ReqruitBarrack);

        var response = await _client.PostAsJsonAsync("api/city/constructBuilding", command);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task TrainUnit_WithValidCommand_ShouldReturnNoContent()
    {
        var cityId = new CityId("city_id");
        var ownerFaction = FactionType.England;
        var template = new UnitTemplate() { Type = UnitType.Peasant, RequiredBuilding = BuildingType.ReqruitBarrack };
        var army = Army.Create(ownerFaction, Vector2.Zero, new UnitTemplate());
        army.RecruitUnit(template);
        var building = Building.CreateWithCustomStatus(
            "test_barrack", BuildingType.ReqruitBarrack, ownerFaction, cityId,
            isConstructed: true, turnsToConstruct: 0
        );

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var faction = new Faction(FactionType.England, template.Cost + 100, PlayerType.Human);
            var cityPreset = new CityPreset(cityId, "London Test", CityType.Village, 1000, []);
            var city = new City(cityPreset, faction.Type, Vector2.Zero);

            city.RegisterBuilding(building);

            context.Cities.Add(city);
            context.Factions.Add(faction);
            context.Armies.Add(army);

            await context.SaveChangesAsync();
        }

        var command = new RecruitUnitCommand(army.Id, template.Type, ownerFaction);

        var response = await _client.PostAsJsonAsync("api/city/trainUnit", command);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task ConstructBuilding_WithInvalidCityId_ShouldReturnBadRequest()
    {
        var command = new ConstructBuildingCommand("x", BuildingType.None);

        var response = await _client.PostAsJsonAsync("api/city/constructBuilding", command);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.NotNull(problemDetails);
        Assert.True(
            problemDetails.Errors.Keys.Any(k => k.Contains("CityId")), "Ключ с ошибкой CityId не найден"
        );
        Assert.True(
            problemDetails.Errors.Keys.Any(k => k.Contains("BuildingType")), "Ключ с ошибкой BuildingType не найден"
        );
    }

    [Fact]
    public async Task CancelBuildingConstruction_ShouldReturnNoContent()
    {
        var buildingId = new BuildingId("test_building");
        var cityId = new CityId("test_london");

        var building = Building.CreateWithCustomStatus(
            buildingId, BuildingType.ReqruitBarrack, FactionType.England, "test_london",
            isConstructed: false,
            turnsToConstruct: 2
        );

        using var scope = await SeedTestWorldAsync(cityId, buildingToRegister: building);

        var response = await _client.DeleteAsync($"api/city/cancelBuildingConstruction_{buildingId}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task CancelUnitTraining_ShouldReturnNoContent()
    {
        var unitCost = 1000;
        var buildingType = BuildingType.ReqruitBarrack;
        var ownerFaction = FactionType.England;
        var cityId = "test_city";
        var building = Building.CreateWithCustomStatus(
            "test_building", buildingType, ownerFaction, cityId,
            isConstructed: true, turnsToConstruct: 0
        );
        var template = new UnitTemplate() { Type = UnitType.Peasant, RequiredBuilding = buildingType };
        var army = Army.Create(ownerFaction, Vector2.Zero, new UnitTemplate());
        army.RecruitUnit(template);

        using (var scope = _factory.Services.CreateScope())
        {
            await SeedTestWorldAsync(cityId, unitCost, buildingToRegister: building);

            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            context.Armies.Add(army);
            await context.SaveChangesAsync();
        }

        var response = await _client.DeleteAsync($"api/city/cancelUnitRecruiting_{army.Units[1].Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task GetConstructionOptions_ShouldReturnOk_AndValidCatalog()
    {
        var cityId = new CityId("london_test");
        var barrackCost = GameBalance.Buildings.GetTemplate(BuildingType.ReqruitBarrack).Cost;

        using var scope = await SeedTestWorldAsync(cityId, barrackCost);

        var response = await _client.GetAsync($"api/city/{cityId}/getConstructionOptions");

        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var catalog = await response.Content.ReadFromJsonAsync<IEnumerable<CityCatalogOptionDto<BuildingType>>>();

        Assert.NotNull(catalog);
        Assert.NotEmpty(catalog);

        var reqruitBarrack = catalog.FirstOrDefault(b => b.Type == BuildingType.ReqruitBarrack);

        Assert.NotNull(reqruitBarrack);
        Assert.Equal(CityCatalogOptionAvailability.Available, reqruitBarrack.Availability);
    }

    [Fact]
    public async Task GetRecruitOptions_ShouldReturnOk_AndValidCatalog()
    {
        var cityId = new CityId("test_london");
        var unitCost = GameBalance.Units.GetTemplate(UnitType.Peasant).Cost;
        var building = Building.CreateWithCustomStatus(
            "test_building", BuildingType.ReqruitBarrack, FactionType.England, cityId,
            isConstructed: true,
            turnsToConstruct: 0
        );

        using var scope = await SeedTestWorldAsync(cityId, unitCost, building);

        var response = await _client.GetAsync($"api/city/{cityId}/getRecruitOptions");

        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var catalog = await response.Content.ReadFromJsonAsync<IEnumerable<CityCatalogOptionDto<UnitType>>>();

        Assert.NotNull(catalog);
        Assert.Contains(catalog, dto => dto.Type == UnitType.Peasant);

        var unit = catalog.First(u => u.Type == UnitType.Peasant);
        Assert.Equal(CityCatalogOptionAvailability.Available, unit.Availability);
    }

    private async Task<IServiceScope> SeedTestWorldAsync(
        CityId cityId, int? entityCost = 0, Building? buildingToRegister = null)
    {
        var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var faction = new Faction(FactionType.England, entityCost ?? 0, PlayerType.Human);
        var cityPreset = new CityPreset(cityId, "London Test", CityType.Settlement, 1000, []);
        var city = new City(cityPreset, faction.Type, new(0f, 0f));

        if (buildingToRegister != null)
        {
            city.RegisterBuilding(buildingToRegister);
        }

        context.Cities.Add(city);
        context.Factions.Add(faction);
        await context.SaveChangesAsync();

        return scope;
    }
}