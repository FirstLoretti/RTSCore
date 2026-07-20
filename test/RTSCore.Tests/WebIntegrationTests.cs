using System.Net;
using System.Net.Http.Json;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using RTSCore.Domain.ValueObjects;
using RTSCore.Infrastructure.Persistence;
using RTSCore.WebApi.Dtos;

namespace RTSCore.Tests;

public class WebIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
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
        Assert.Contains("Id не может быть пустым или состоять из пробелов", idErrors);
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

    private readonly HttpClient _client;
    private readonly SqliteConnection _sqliteConnection;

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

        var bootstrapedFactory = factory.WithWebHostBuilder(builder => builder.ConfigureServices(
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

        _client = bootstrapedFactory.CreateClient();

        using var scope = bootstrapedFactory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.EnsureCreated();
    }
}