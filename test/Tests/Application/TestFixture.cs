using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Npgsql;

using Respawn;

using RTSCore.Application.Campaign.Lifecycle;
using RTSCore.Application.Repositories;
using RTSCore.Domain.Configurations;
using RTSCore.Domain.Entities.Armies;
using RTSCore.Domain.Entities.Armies.Combat;
using RTSCore.Infrastructure.Persistence;

using Testcontainers.PostgreSql;

namespace Tests.Application;

public sealed class TestFixture : IAsyncLifetime
{
    public IServiceProvider ServiceProvider { get; private set; } = null!;

    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:15-alpine")
        .WithDatabase("test_db")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    private Respawner _respawner = null!;
    private NpgsqlConnection _connection = null!;

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        var services = new ServiceCollection();
        var connectionString = _container.GetConnectionString();

        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
        services.AddLogging();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(StartCampaignCommanHandler).Assembly));
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();
        services.AddSingleton<AutoBattleCalculator>();
        services.AddSingleton<UnitStatsCalculator>();
        RegisterGameConfigs(services);

        ServiceProvider = services.BuildServiceProvider();

        using (var scope = ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await context.Database.EnsureCreatedAsync();
        }

        _connection = new NpgsqlConnection(connectionString);
        await _connection.OpenAsync();

        _respawner = await Respawner.CreateAsync(_connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = ["public"]
        });
    }

    public async Task DisposeAsync()
    {
        await _connection.DisposeAsync();
        await _container.StopAsync();
        await _container.DisposeAsync();
    }

    public async Task ResetDatabaseAsync() => await _respawner.ResetAsync(_connection);

    private static void RegisterGameConfigs(ServiceCollection services)
    {
        var gameConfig = new GameConfigurations()
        {
            WorldConfiguration = TestData.WorldConfiguration,

            Units = TestData.UnitCatalog,
            Factions = TestData.FactionCatalog,
            Buildings = TestData.BuildingCatalog,
            Cities = TestData.CityCatalog,
            AiPersonalities = TestData.AiCatalog,

            Movement = TestData.MovementConfiguration,
            Diplomacy = TestData.DiplomacyConfig,
            AutoBattle = TestData.AutoBattleConfiguration,
            UnitConfiguration = TestData.UnitConfiguration,
            CityConfiguration = TestData.CityConfiguration
        };

        services.AddSingleton(gameConfig);

        foreach (var property in typeof(GameConfigurations).GetProperties())
        {
            var value = property.GetValue(gameConfig);

            ArgumentNullException.ThrowIfNull(value, property.Name);

            services.AddSingleton(property.PropertyType, value);
        }
    }
}