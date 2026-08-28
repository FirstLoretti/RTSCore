using MediatR;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using RTSCore.Application.Cities.Commands;
using RTSCore.Application.Campaing.Commands;
using RTSCore.Application.Common.Behaviors;
using RTSCore.Application.Units.Commands;
using RTSCore.Application.Units.Queries;
using RTSCore.Domain.Entities;
using RTSCore.Domain.Exeptions;
using RTSCore.Domain.Interfaces;
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

    [Fact]
    public async Task Mediator_EndTurn_ShouldAdvanceConstruction_AndMarkAsConstructed()
    {
        var (dbName, serviceProvider) = Arrange();

        var buildingId = new BuildingId("test_building");

        using (var scope = serviceProvider.CreateScope())
        {
            var cityPreset = new CityPreset("test_london", "Test_London", CityType.Town, 1000, []);
            var city = new City(cityPreset, FactionType.England);
            var buildingTemplate = new BuildingTemplate(BuildingType.Barrack, "Test_Barrack", 1000, 1);
            var building = new Building(buildingId, BuildingType.Barrack, FactionType.England, city.Id);

            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Cities.Add(city);
            context.Buildings.Add(building);

            await context.SaveChangesAsync();
        }

        using (var scope = serviceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            await mediator.Send(new EndTurnCommand());
        }

        Building dbBuilding;

        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            dbBuilding = await context.Buildings.FirstAsync(b => b.Id == buildingId);
        }

        DeleteDatabase(dbName);

        Assert.True(dbBuilding.IsConstructed);
        Assert.Equal(0, dbBuilding.TurnsToConstruct);
    }
    #endregion

    #region CityCommands

    [Theory]
    [InlineData(5000, true)]
    [InlineData(0, false)]
    public async Task Mediator_ConstructBuilding_ShouldHandleGoldValidationCorrectly(int initalGold, bool shouldSucceed)
    {
        var (dbName, serviceProvider) = Arrange();

        var buildingTemplate = new BuildingTemplate(BuildingType.Barrack, "Test Barrack", 1000, 1);
        var cityId = new CityId("test_london");
        var factionType = FactionType.England;

        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var player = new Faction(factionType, initalGold, PlayerType.Human);
            var cityPreset = new CityPreset(cityId, "Test London", CityType.Town, 1500, []);
            var city = new City(cityPreset, factionType);

            context.Factions.Add(player);
            context.Cities.Add(city);

            await context.SaveChangesAsync();
        }

        using (var scope = serviceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var command = new ConstructBuildingCommand(cityId, buildingTemplate.Type);

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

        if (shouldSucceed)
        {
            Assert.Equal(initalGold - buildingTemplate.Cost, dbFaction.Gold);
            Assert.Single(dbBuildings);
            Assert.Equal($"building_{cityId}_{buildingTemplate.Type.ToString().ToLower()}", dbBuildings.First().Id);
        }
        else
        {
            Assert.Equal(initalGold, dbFaction.Gold);
            Assert.Empty(dbBuildings);
        }
    }

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public async Task Mediator_CancelConstruction_ShouldHandleRulesCorrectly(bool isAlreadyConstructed, bool shouldSucceed)
    {
        var buildingId = "test_building";
        var factionType = FactionType.England;
        var initialGold = 1000;
        var cityId = "test_london";
        var buildingTemplate = new BuildingTemplate(BuildingType.Barrack, "Barrack", 1000, 1);

        var (dbName, serviceProvider) = Arrange();

        using (var scope = serviceProvider.CreateScope())
        {
            var cityPreset = new CityPreset(cityId, "Test London", CityType.Town, 1500, []);
            var city = new City(cityPreset, factionType);
            var building = Building.CreateWithCustomStatusForTests(
                buildingId,
                BuildingType.Barrack,
                factionType,
                cityId,
                isAlreadyConstructed,
                isAlreadyConstructed ? 0 : 1
            );
            var goldAfterConstruction = initialGold - buildingTemplate.Cost;
            var faction = new Faction(factionType, goldAfterConstruction, PlayerType.Human);

            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Cities.Add(city);
            context.Buildings.Add(building);
            context.Factions.Add(faction);

            await context.SaveChangesAsync();
        }

        using (var scope = serviceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var command = new CancelConstructBuildingCommand(buildingId);

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
            dbFaction = await context.Factions.FirstAsync(f => f.Type == factionType);
            dbBuildings = await context.Buildings.ToListAsync();
        }

        DeleteDatabase(dbName);

        var finalGold = initialGold - buildingTemplate.Cost + buildingTemplate.Cost / 2;

        if (shouldSucceed)
        {
            Assert.Equal(finalGold, dbFaction.Gold);
            Assert.Empty(dbBuildings);
        }
        else
        {
            Assert.Equal(initialGold - buildingTemplate.Cost, dbFaction.Gold);
            Assert.Single(dbBuildings);
            Assert.True(dbBuildings.First().IsConstructed);
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