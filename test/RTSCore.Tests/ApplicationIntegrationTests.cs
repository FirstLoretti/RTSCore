using MediatR;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using RTSCore.Application.Buildings.Commands;
using RTSCore.Application.Campaing.Commands;
using RTSCore.Application.Common.Behaviors;
using RTSCore.Application.Units.Commands;
using RTSCore.Application.Units.Queries;
using RTSCore.Domain.Entities;
using RTSCore.Domain.Exeptions;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.Services;
using RTSCore.Domain.ValueObjects;
using RTSCore.Domain.ValueObjects.Presets;
using RTSCore.Infrastructure.Persistence;

using Unit = RTSCore.Domain.Entities.Unit;

namespace RTSCore.Tests;

public class ApplicationIntegrationTests
{
    #region UnitCommands

    [Fact]
    public async Task Mediator_ShouldRouteCreateAndGetUnitCommandToHandlers()
    {
        var (dbName, serviceProvider) = Arrange();

        UnitId dbUnitId;
        using (var scope = serviceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var command = new CreateUnitCommand("unit", UnitType.EnglandSwordman, FactionType.England);

            dbUnitId = await mediator.Send(command);
        }

        Unit? dbUnit;
        using (var scope = serviceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var request = new GetUnitQuery(dbUnitId);

            dbUnit = await mediator.Send(request);
        }

        DeleteDatabase(dbName);

        Assert.NotNull(dbUnit);
        Assert.Equal(dbUnitId, dbUnit.Id);
    }

    [Fact]
    public async Task Mediator_ShouldRouteAddExperienceCommandToHandler_AndChageLvlAndExp()
    {
        var (dbName, serviceProvider) = Arrange();

        UnitId dbUnitId;
        using (var scope = serviceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var command = new CreateUnitCommand("unit", UnitType.EnglandSwordman, FactionType.England);
            dbUnitId = await mediator.Send(command);
        }

        int finalLevel, finalExperience;
        using (var scope = serviceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var command = new AddExperienceCommand(dbUnitId, int.MaxValue);
            (finalLevel, finalExperience) = await mediator.Send(command);
        }

        DeleteDatabase(dbName);

        Assert.True(finalLevel > 1);
        Assert.True(finalExperience > 0);
    }

    #endregion

    #region CampaingCommands

    [Theory]
    [InlineData(new[] { FactionType.England }, PlayerType.Human, PlayerType.Ai)]
    [InlineData(new[] { FactionType.England, FactionType.France }, PlayerType.Human, PlayerType.Human)]
    public async Task Mediator_StartCampaingCommand_ShouldSaveCorrectEntitiesToDatabase(
        FactionType[] selectedFactions,
        PlayerType englandPlayerType,
        PlayerType francePlayerType
    )
    {
        var (dbName, serviceProvider) = Arrange(services =>
        {
            var factionPresets = new FactionPreset[]
            {
                new(
                    Type: FactionType.England,
                    Gold: 5000,
                    Cities:
                    [
                        new CityPreset("test_london","Test London",CityType.Town,1000, [BuildingType.Barrack])
                    ]
                ),
                new(
                    Type: FactionType.France,
                    Gold: 7500,
                    Cities:
                    [
                        new CityPreset("test_paris", "Test Paris", CityType.Village, 500, [BuildingType.Market])
                    ]
                )
            };

            services.AddSingleton(factionPresets);
        });

        using (var scope = serviceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var command = new StartCampaignCommand(selectedFactions);
            await mediator.Send(command);
        }

        List<Faction> players;
        List<Building> buildings;
        List<City> cities;

        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            players = await context.Factions.ToListAsync();
            buildings = await context.Buildings.ToListAsync();
            cities = await context.Cities.ToListAsync();
        }

        DeleteDatabase(dbName);

        Assert.Equal(2, players.Count);
        Assert.Equal(englandPlayerType, players.First(p => p.Type == FactionType.England).PlayerType);
        Assert.Equal(francePlayerType, players.First(p => p.Type == FactionType.France).PlayerType);

        var london = cities.First(c => c.Id == "test_london");
        Assert.Equal(1000, london.Population);
        Assert.Contains(buildings, b => b.Type == BuildingType.Barrack && b.CityId == london.Id);
    }

    #endregion

    #region CityCommands

    [Theory]
    [InlineData(5000, true)]
    [InlineData(0, false)]
    public async Task Mediator_ConstructBuilding_ShouldHandleGoldValidationCorrectly(int initalGold, bool shouldSucceed)
    {
        var (dbName, serviceProvider) = Arrange();

        var buildingType = BuildingType.Barrack;
        var cityId = new CityId("test_london");
        var factionType = FactionType.England;

        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var player = new Faction(factionType, initalGold, PlayerType.Human);
            var cityPreset = new CityPreset(cityId, "Test London", CityType.Town, 1000, []);
            var city = new City(cityPreset, factionType);

            context.Factions.Add(player);
            context.Cities.Add(city);

            await context.SaveChangesAsync();
        }

        using (var scope = serviceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var command = new ConstructBuildingCommand(cityId, buildingType);

            if (shouldSucceed)
            {
                await mediator.Send(command);
            }
            else
            {
                await Assert.ThrowsAsync<GameRuleException>(async () => await mediator.Send(command));
            }
        }

        Faction dbFaction;
        List<Building> dbBuildings;

        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            dbFaction = await context.Factions.FirstAsync(p => p.Type == factionType);
            dbBuildings = await context.Buildings.ToListAsync();
        }

        DeleteDatabase(dbName);

        var building = GameBalance.Buildings.GetTemplate(buildingType);

        if (shouldSucceed)
        {
            Assert.Equal(initalGold - building.Cost, dbFaction.Gold);
            Assert.Single(dbBuildings);
            Assert.Equal($"building_{cityId}_{buildingType.ToString().ToLower()}", dbBuildings.First().Id);
        }
        else
        {
            Assert.Equal(initalGold, dbFaction.Gold);
            Assert.Empty(dbBuildings);
        }
    }

    #endregion

    #region Shared

    private static (string, ServiceProvider) Arrange(Action<IServiceCollection>? configure = null)
    {
        string dbName = "app_test.db";

        File.Delete(dbName);

        var services = new ServiceCollection();

        services.AddLogging();
        services.AddDbContext<AppDbContext>(
            options => options.UseSqlite($"Data Source={dbName}")
        );
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();
        services.AddScoped<IUnitRepository, SqlUnitRepository>();
        services.AddScoped<ICityRepository, SqlCityRepository>();
        services.AddScoped<IFactionRepository, SqlFactionRepository>();
        services.AddScoped<IBuildingRepository, SqlBuildingRepository>();

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(
            typeof(CreateUnitCommand).Assembly
        ));

        configure?.Invoke(services);

        var serviceProvider = services.BuildServiceProvider();

        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.EnsureCreated();

        return (dbName, serviceProvider);
    }

    private static void DeleteDatabase(string name)
    {
        SqliteConnection.ClearAllPools();
        File.Delete(name);
    }

    #endregion
}