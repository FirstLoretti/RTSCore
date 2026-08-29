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
            $"Юнита UnitId {{ Value = test_invulnerable }} " +
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

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var faction = new Faction(FactionType.England, 5000, PlayerType.Human);
            var cityPreset = new CityPreset(cityId, "Test_London", CityType.Settlement, 1000, []);
            var city = new City(cityPreset, faction.Type);

            context.Factions.Add(faction);
            context.Cities.Add(city);

            await context.SaveChangesAsync();
        }

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

        using (var scope = _factory.Services.CreateScope())
        {

            var cityPreset = new CityPreset("test_london", "Test London", CityType.Settlement, 1500, []);
            var city = new City(cityPreset, FactionType.England);

            var building = Building.CreateWithCustomStatusForTests(
                buildingId,
                BuildingType.ReqruitBarrack,
                FactionType.England,
                "test_london",
                false,
                2
            );

            var faction = new Faction(FactionType.England, 1000, PlayerType.Human);

            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Buildings.Add(building);
            context.Factions.Add(faction);
            context.Cities.Add(city);

            await context.SaveChangesAsync();
        }

        var responce = await _client.DeleteAsync($"api/city/cancelBuildingConstruction_{buildingId}");

        Assert.Equal(HttpStatusCode.NoContent, responce.StatusCode);
    }

    [Fact]
    public async Task GetConstructionOptions_ShouldReturnOk_AndValidCatalog()
    {
        var cityId = new CityId("london_test");

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var faction = new Faction(FactionType.England, 1000, PlayerType.Human);
            var cityPreset = new CityPreset(cityId, "London Test", CityType.Settlement, 1500, []);
            var city = new City(cityPreset, faction.Type);

            context.Cities.Add(city);
            context.Factions.Add(faction);

            await context.SaveChangesAsync();
        }

        var responce = await _client.GetAsync($"api/city/{cityId}/getConstructionOptions");

        Assert.NotNull(responce);
        Assert.Equal(HttpStatusCode.OK, responce.StatusCode);

        var catalog = await responce.Content.ReadFromJsonAsync<IEnumerable<ConstructionOptionDto>>();

        Assert.NotNull(catalog);
        Assert.Equal(2, catalog.Count());

        var reqruitBarrack = catalog.FirstOrDefault(b => b.Type == BuildingType.ReqruitBarrack);
        Assert.NotNull(reqruitBarrack);
    }

    #endregion

    #region Shared

    private readonly HttpClient _client;
    private readonly SqliteConnection _sqliteConnection;
    private readonly WebApplicationFactory<Program> _factory;

    public void Dispose()
    {
        _sqliteConnection.Close();
        _sqliteConnection.Dispose();
        _client.Dispose();

        GC.SuppressFinalize(this);
    }

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

    #endregion
}