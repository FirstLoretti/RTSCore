using System.Net;
using System.Net.Http.Json;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using RTSCore.Domain.ValueObjects;
using RTSCore.Infrastructure.Persistence;
using RTSCore.WebApi.Dtos;

namespace RTSCore.Tests;

public class WebIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public WebIntegrationTests(WebApplicationFactory<Program> factory)
    {
        var bootstrapedFactory = factory.WithWebHostBuilder(builder => builder.ConfigureServices(
                services =>
                {
                    var descriptor = services.SingleOrDefault(d =>
                        d.ServiceType == typeof(DbContextOptions<AppDbContext>)
                    );
                    if (descriptor != null) services.Remove(descriptor);

                    services.AddDbContext<AppDbContext>(options =>
                        options.UseSqlite("Data Source=InMemoryTestDb.db;Mode=Memory;Cache=Shared"));
                }));

        _client = bootstrapedFactory.CreateClient();

        using var scope = bootstrapedFactory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.EnsureCreated();
    }

    [Fact]
    public async Task Create_ShouldHandleInvalidDto_AndReturnBadeRequestResponce()
    {
        var invalidDto = new UnitCreateDto("1", UnitType.EnglandSwordman, FactionType.England);

        var response = await _client.PostAsJsonAsync("api/unit", invalidDto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.NotNull(problemDetails);
        Assert.Equal("Ошибка валидации данных", problemDetails.Title);
        Assert.True(problemDetails.Errors.ContainsKey("Id"), "Ответ должен содержать ошибку для поля 'Id'");
    }
}