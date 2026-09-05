using System.Net;
using System.Net.Http.Json;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using RTSCore.Application.Cities.Commands;
using RTSCore.Application.Campaing.Commands;
using RTSCore.Domain.Entities;
using RTSCore.Domain.ValueObjects;
using RTSCore.Domain.ValueObjects.Presets;
using RTSCore.Infrastructure.Persistence;
using RTSCore.Domain.Services;
using RTSCore.Application.Cities.Queries.Common;
using RTSCore.Application.Units.Commands;
using RTSCore.Application.Campaing.Commands.Diplomacy;
namespace RTSCore.Tests;

public class WebIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    #region DiplomacyController

    [Fact]
    public async Task DeclareWar_WithValidCommand_ShouldReturnNoContent()
    {
        var initiator = FactionType.England;
        var target = FactionType.France;

        using (var scope = _factory.Services.CreateScope())
        {
            var england = new Faction(initiator, 0, PlayerType.Human);
            var france = new Faction(target, 0, PlayerType.Ai);
            var relation = new DiplomacyRelation(initiator, target, 0);
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            context.Factions.Add(england);
            context.Factions.Add(france);
            context.DiplomacyRelations.Add(relation);

            await context.SaveChangesAsync();
        }

        var command = new DeclareWarCommand(initiator, target);
        var response = await _client.PostAsJsonAsync("api/diplomacy/offers/war", command);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task SendPeaceOffer_WithValidCommand_ShouldReturnOkWithGuid()
    {
        var initiator = FactionType.England;
        var target = FactionType.France;

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var england = new Faction(initiator, 0, PlayerType.Human);
            var france = new Faction(target, 0, PlayerType.Ai);
            var relation = new DiplomacyRelation(initiator, target, 0);
            relation.DeclareWar();

            context.Factions.Add(england);
            context.Factions.Add(france);
            context.DiplomacyRelations.Add(relation);

            await context.SaveChangesAsync();
        }

        var command = new SendPeaceOfferCommand(initiator, target);
        var response = await _client.PostAsJsonAsync("api/diplomacy/offers/peace", command);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var offerId = await response.Content.ReadFromJsonAsync<Guid>();
        Assert.NotEqual(Guid.Empty, offerId);
    }

    [Fact]
    public async Task SendTradeOffer_WithValidCommand_ShouldReturnOkWithGuid()
    {

        var initiator = FactionType.England;
        var target = FactionType.France;

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var england = new Faction(initiator, 0, PlayerType.Human);
            var france = new Faction(target, 0, PlayerType.Ai);
            var relation = new DiplomacyRelation(initiator, target, 0);

            context.Factions.Add(england);
            context.Factions.Add(france);
            context.DiplomacyRelations.Add(relation);

            await context.SaveChangesAsync();
        }

        var command = new SendTradeOfferCommand(initiator, target);
        var response = await _client.PostAsJsonAsync("api/diplomacy/offers/trade", command);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var offerId = await response.Content.ReadFromJsonAsync<Guid>();
        Assert.NotEqual(Guid.Empty, offerId);
    }

    [Fact]
    public async Task AcceptOffer_WithValidCommand_ShouldReturnNoContent()
    {
        var initiator = FactionType.England;
        var target = FactionType.France;
        Guid offerId;

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var england = new Faction(initiator, 0, PlayerType.Human);
            var france = new Faction(target, 0, PlayerType.Ai);
            var relation = new DiplomacyRelation(initiator, target, GameBalance.Diplomacy.MinStandingForTrade);
            var offer = new DiplomacyOffer(initiator, target, OfferType.TradeAgreement);
            offerId = offer.Id;

            context.Factions.Add(england);
            context.Factions.Add(france);
            context.DiplomacyRelations.Add(relation);
            context.DiplomacyOffers.Add(offer);
            await context.SaveChangesAsync();
        }

        var command = new AcceptOfferCommand(offerId);
        var response = await _client.PostAsJsonAsync($"api/diplomacy/offers/{offerId}/accept", command);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task RejectOffer_WithValidCommand_ShouldReturnNoContent()
    {
        var initiator = FactionType.England;
        var target = FactionType.France;
        Guid offerId;

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var england = new Faction(initiator, 0, PlayerType.Ai);
            var france = new Faction(target, 0, PlayerType.Human);
            var relation = new DiplomacyRelation(initiator, target, GameBalance.Diplomacy.InitialStanding);
            var offer = new DiplomacyOffer(initiator, target, OfferType.TradeAgreement);
            offerId = offer.Id;

            context.Factions.Add(england);
            context.Factions.Add(france);
            context.DiplomacyRelations.Add(relation);
            context.DiplomacyOffers.Add(offer);
            await context.SaveChangesAsync();
        }

        var command = new RejectOfferCommand(offerId);
        var response = await _client.PostAsJsonAsync($"api/diplomacy/offers/{offerId}/reject", command);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    #endregion

    #region UnitController

    [Fact]
    public async Task DisbandUnit_WithValidCommand_ShoulReturnNoContent()
    {
        var unitId = new UnitId("test_unit");

        using (var scope = _factory.Services.CreateScope())
        {
            var template = new UnitTemplate(
                UnitType.EnglandMilitia, "Test Unit", 1, 1, 1, 1, 1, 1, 1, 1,
                TurnsToRecruit: 0
            );
            var unit = Unit.CreateWithCustomStatus(unitId, FactionType.England, template, turnsToRecruit: 0);

            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            context.Units.Add(unit);
            await context.SaveChangesAsync();
        }

        var response = await _client.DeleteAsync($"api/unit/{unitId}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Disband_ShouldReturn422_WhenUnitInvulnerable()
    {
        var unitId = "test_invulnerable";

        using (var scope = _factory.Services.CreateScope())
        {
            var unit = new Unit(unitId, FactionType.England, GameBalance.Units.GetTemplate(UnitType.Invulnerable));
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            context.Add(unit);
            await context.SaveChangesAsync();
        }

        var response = await _client.DeleteAsync($"api/unit/{unitId}");

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);

        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.NotNull(problemDetails);
        Assert.Equal("Нарушение игровых правил", problemDetails.Title);
        Assert.Equal(
            $"[{nameof(DisbandUnitCommand)}] " +
            $"Юнита {unitId} с типом {UnitType.Invulnerable} нельзя удалить из базы данных",
            problemDetails.Detail
        );
    }


    #endregion

    #region CampaingController

    [Fact]
    public async Task StartCampaign_WithValidCommand_ShouldReturnNoContent()
    {
        var command = new StartCampaignCommand([FactionType.England]);

        var response = await _client.PostAsJsonAsync("api/campaign/start", command);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task StartCampaing_WithEmptyFactions_ShouldReturnBadRequest()
    {
        var command = new StartCampaignCommand([]);

        var response = await _client.PostAsJsonAsync("api/campaign/start", command);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task EndTurn_ShouldReturnNoContent()
    {
        var responce = await _client.PostAsJsonAsync("api/campaign/endTurn", new EndTurnCommand());

        Assert.Equal(HttpStatusCode.NoContent, responce.StatusCode);
    }

    #endregion

    #region CityController

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
        var cityId = new CityId("test_london");
        var unitType = UnitType.EnglandPeasant;
        var unitCost = GameBalance.Units.GetTemplate(unitType).Cost;
        var ownerFaction = FactionType.England;
        var building = Building.CreateWithCustomStatus(
            "test_barrack", BuildingType.ReqruitBarrack, ownerFaction, cityId,
            isConstructed: true, turnsToConstruct: 0
        );

        using var scope = await SeedTestWorldAsync(cityId, unitCost, building);

        var command = new RecruitUnitCommand(cityId, unitType, ownerFaction);

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
        var unitId = new UnitId("test_unit");
        var cityId = "test_city";
        var building = Building.CreateWithCustomStatus(
            "test_building", buildingType, ownerFaction, cityId,
            isConstructed: true, turnsToConstruct: 0
        );

        using var scope = await SeedTestWorldAsync(cityId, unitCost, buildingToRegister: building);

        var template = new UnitTemplate(
            UnitType.EnglandPeasant, "Test Peasant", unitCost, 1, 1, 1, 1, 1, 1, 1,
            TurnsToRecruit: 1, RequiredBuilding: buildingType);
        var unit = Unit.CreateWithCustomStatus(
            unitId, ownerFaction, template,
            turnsToRecruit: 1, currentCityId: cityId
        );

        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Units.Add(unit);
        await context.SaveChangesAsync();

        var response = await _client.DeleteAsync($"api/city/cancelUnitRecruiting_{unitId}");

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
        var unitCost = GameBalance.Units.GetTemplate(UnitType.EnglandPeasant).Cost;
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
        Assert.Contains(catalog, dto => dto.Type == UnitType.EnglandPeasant);

        var unit = catalog.First(u => u.Type == UnitType.EnglandPeasant);
        Assert.Equal(CityCatalogOptionAvailability.Available, unit.Availability);
    }

    #endregion

    #region Common

    private readonly HttpClient _client;
    private readonly SqliteConnection _sqliteConnection;
    private readonly WebApplicationFactory<Program> _factory;

    public WebIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _sqliteConnection = new SqliteConnection("Data Source=:memory:");
        _sqliteConnection.Open();

        _factory = factory.WithWebHostBuilder(builder => builder.ConfigureServices(
                services =>
                {
                    var descriptor = services.SingleOrDefault(d =>
                        d.ServiceType == typeof(DbContextOptions<AppDbContext>)
                    );

                    Assert.NotNull(descriptor);

                    services.Remove(descriptor);

                    services.AddDbContext<AppDbContext>(options =>
                        options.UseSqlite(_sqliteConnection));
                }));

        _client = _factory.CreateClient();

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _sqliteConnection.Close();
        _sqliteConnection.Dispose();
        _client.Dispose();

        GC.SuppressFinalize(this);
    }

    private async Task<IServiceScope> SeedTestWorldAsync(CityId cityId, int? entityCost = 0, Building? buildingToRegister = null)
    {
        var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var faction = new Faction(FactionType.England, entityCost ?? 0, PlayerType.Human);
        var cityPreset = new CityPreset(cityId, "London Test", CityType.Settlement, 1000, []);
        var city = new City(cityPreset, faction.Type);

        if (buildingToRegister != null)
        {
            city.RegisterBuilding(buildingToRegister);
        }

        context.Cities.Add(city);
        context.Factions.Add(faction);

        await context.SaveChangesAsync();

        return scope;
    }

    #endregion
}