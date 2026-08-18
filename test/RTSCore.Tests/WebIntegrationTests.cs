using System.Net;
using System.Net.Http.Json;

using FluentValidation;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using RTSCore.Application.Buildings.Commands;
using RTSCore.Domain.Entities;
using RTSCore.Domain.ValueObjects;
using RTSCore.Infrastructure.Persistence;
using RTSCore.WebApi.Dtos;

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

        var idErrors = problemDetails.Errors["Id"];
        var unitTypeErrors = problemDetails.Errors["Type"];
        var factionTypeErrors = problemDetails.Errors["Faction"];

        Assert.Equal("Ошибка валидации данных", problemDetails.Title);
        Assert.True(problemDetails.Errors.ContainsKey("Id"), "Ответ должен содержать ошибку для поля 'Id'");
        Assert.True(problemDetails.Errors.ContainsKey("Type"), "Ответ должен содержать ошибку для поля 'Type'");
        Assert.True(problemDetails.Errors.ContainsKey("Faction"), "Ответ должен содержать ошибку для поля 'Faction'");
        Assert.Contains("Id не может быть пустым", idErrors);
        Assert.Contains("Неверный тип юнита", unitTypeErrors);
        Assert.Contains("Неверный тип фракции", factionTypeErrors);
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

        var amountError = problemDetails.Errors["Amount"];

        Assert.Equal("Ошибка валидации данных", problemDetails.Title);
        Assert.True(problemDetails.Errors.ContainsKey("Amount"), "Ответ должен содержать ошибку для поля 'Amount'");
        Assert.Contains("Начисляемый опыт должен быть в районе 0-5000", amountError);
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

    #region BuildingController
    [Fact]
    public async Task Train_ShouldAddUnitToQueue_AndReturnNoContent_WhenDataIsValid()
    {
        var barrackId = "barrack_testId";

        using (var scope = _factory.Services.CreateScope())
        {
            var buildintTemplate = new BuildingTemplate(
                BuildingType.EnglandBarrack,
                FactionType.England,
                "Test Barrack",
                1,
                1
            );
            var barrack = new Barrack(barrackId, buildintTemplate);

            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Set<Barrack>().Add(barrack);

            await context.SaveChangesAsync();
        }

        var command = new TrainUnitCommand(barrackId, "test_unit");

        var response = await _client.PostAsJsonAsync("api/building/train", command);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var barrack = await context.Buildings.FindAsync(new BuildingId(barrackId));

            Assert.NotNull(barrack);
            Assert.Equal(1, ((Barrack)barrack).ActiveRecruitmentSlots);
        }
    }

    [Fact]
    public async Task Train_ShouldReturn400_WhenBuildingIdIsTooShort()
    {
        var command = new TrainUnitCommand("1", "unit_mock");

        var response = await _client.PostAsJsonAsync("api/building/train", command);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.NotNull(problemDetails);
        Assert.Equal("Ошибка валидации данных", problemDetails.Title);
        Assert.True(problemDetails.Errors.ContainsKey("BuildingId"));
    }

    [Fact]
    public async Task Train_ShouldReturn422_WhenBarrackQueueIsFull()
    {
        var barrackId = $"barrack_{Guid.NewGuid().ToString()[..10]}";

        using (var scope = _factory.Services.CreateScope())
        {
            var buildintTemplate = new BuildingTemplate(
                BuildingType.EnglandBarrack,
                FactionType.England,
                "Test Barrack",
                1,
                1
            );
            var barrack = new Barrack(barrackId, buildintTemplate);

            barrack.AddUnitToQueue();

            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Set<Barrack>().Add(barrack);

            await context.SaveChangesAsync();
        }

        var command = new TrainUnitCommand(barrackId, "unit_mock");

        var response = await _client.PostAsJsonAsync("api/building/train", command);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);

        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.NotNull(problemDetails);
        Assert.Equal("Нарушение игровых правил", problemDetails.Title);
        Assert.Contains("не имеет свободных слотов под найм", problemDetails.Detail);
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