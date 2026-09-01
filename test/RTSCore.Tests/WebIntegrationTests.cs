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
using RTSCore.WebApi.Dtos;
using RTSCore.Domain.Services;
using RTSCore.Application.Cities.Queries;

namespace RTSCore.Tests;

public class WebIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    #region UnitController

    [Fact]
    public async Task Create_ShouldHandleInvalidDto_AndReturnBadRequestResponse()
    {
        var invalidDto = new UnitCreateDto("", (UnitType)999, (FactionType)999);

        var response = await _client.PostAsJsonAsync("api/unit", invalidDto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.NotNull(problemDetails);

        Assert.Equal("Ошибка валидации данных", problemDetails.Title);
        Assert.True(problemDetails.Errors.ContainsKey("Id"));
        Assert.True(problemDetails.Errors.ContainsKey("Type"));
        Assert.True(problemDetails.Errors.ContainsKey("Faction"));
    }

    [Fact]
    public async Task Get_ShouldHandleInvalidDto_AndReturnNotFoundResponse()
    {
        var response = await _client.GetAsync($"api/unit/{999}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.NotNull(problemDetails);
        Assert.Equal("Сущность не найдена", problemDetails.Title);
    }

    [Fact]
    public async Task AddExperience_ShouldHandleInvalidDto_AndReturnBadRequest()
    {
        var dto = new ExperienceAddDto(int.MaxValue);

        var response = await _client.PostAsJsonAsync($"api/unit/{"test"}/experience", dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.NotNull(problemDetails);

        Assert.Equal("Ошибка валидации данных", problemDetails.Title);
        Assert.True(problemDetails.Errors.ContainsKey("Amount"));
    }

    [Fact]
    public async Task Delete_ShouldReturn422_WhenUnitInvulnerable()
    {
        var unitId = "test_invulnerable";

        using (var scope = _factory.Services.CreateScope())
        {
            var unit = new Unit(unitId, UnitType.Invulnerable, FactionType.England);
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
            $"Юнита {unitId} " +
            $"с типом {UnitType.Invulnerable} нельзя удалить из базы данных",
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

        await SeedTestWorld(cityId, barrackCost);

        var command = new ConstructBuildingCommand(cityId, BuildingType.ReqruitBarrack);

        var response = await _client.PostAsJsonAsync("api/city/constructBuilding", command);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task ConstructBuilding_WithInvalidCommand_ShouldReturnBadRequest()
    {
        var command = new ConstructBuildingCommand("", BuildingType.None);

        var response = await _client.PostAsJsonAsync("api/city/constructBuilding", command);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.NotNull(problemDetails);
        Assert.True(problemDetails.Errors.ContainsKey("CityId"));
        Assert.True(problemDetails.Errors.ContainsKey("BuildingType"));
    }

    [Fact]
    public async Task CancelBuildingConstruction_ShouldReturnNoContent()
    {
        var buildingId = new BuildingId("test_building");
        var cityId = new CityId("test_london");

        var building = Building.CreateWithCustomStatusForTests(
            buildingId, BuildingType.ReqruitBarrack, FactionType.England, "test_london",
            isConstructed: false,
            turnsToConstruct: 2
        );

        await SeedTestWorld(cityId, buildingToRegister: building);

        var response = await _client.DeleteAsync($"api/city/cancelBuildingConstruction_{buildingId}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task GetConstructionOptions_ShouldReturnOk_AndValidCatalog()
    {
        var cityId = new CityId("london_test");
        var barrackCost = GameBalance.Buildings.GetTemplate(BuildingType.ReqruitBarrack).Cost;

        await SeedTestWorld(cityId, barrackCost);

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
        var unitCost = GameBalance.Units.GetTemplate(UnitType.EnglandSwordman).Cost;

        await SeedTestWorld(cityId, unitCost);

        var response = await _client.GetAsync($"api/city/{cityId}/getRecruitOptions");

        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var catalog = await response.Content.ReadFromJsonAsync<IEnumerable<CityCatalogOptionDto<UnitType>>>();

        Assert.NotNull(catalog);
        Assert.Contains(catalog, dto => dto.Type == UnitType.EnglandSwordman);

        var unit = catalog.First(u => u.Type == UnitType.EnglandSwordman);
        Assert.Equal(CityCatalogOptionAvailability.Available, unit.Availability);
    }

    #endregion

    #region Shared

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

    private async Task SeedTestWorld(CityId cityId, int? entityCost = 0, Building? buildingToRegister = null)
    {
        using var scope = _factory.Services.CreateScope();

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
    }

    #endregion
}