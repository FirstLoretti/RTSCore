using MediatR;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using RTSCore.Application.Campaign.Services.Diplomacy;
using RTSCore.Application.Cities.Commands;
using RTSCore.Application.Common.Behaviors;
using RTSCore.Application.Common.Settings;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.Interfaces.Authentication;
using RTSCore.Domain.Services;
using RTSCore.Infrastructure.Authentication;
using RTSCore.Infrastructure.Persistence;

namespace RTSCore.Tests.Base;

public abstract class TestBase : IDisposable
{
    private string? _dbName = null;

    protected ServiceProvider SetupTestInvironment(Action<IServiceCollection>? configure = null)
    {
        _dbName = $"app_test_{Guid.NewGuid():N}.db";

        var testJwtSettings = new JwtSettings
        {
            Secret = "qwperpw_qwerwe1_qwerllvpbp0_`ppp_ppp2lvO_2Q_P`NMMMa",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            ExpiryInMinutes = 60
        };

        var services = new ServiceCollection();

        services.AddLogging();
        services.AddDbContext<AppDbContext>(
            options => options.UseSqlite($"Data Source={_dbName}")
        );
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();
        services.AddScoped<IUnitRepository, SqlUnitRepository>();
        services.AddScoped<ICityRepository, SqlCityRepository>();
        services.AddScoped<IFactionRepository, SqlFactionRepository>();
        services.AddScoped<IBuildingRepository, SqlBuildingRepository>();
        services.AddScoped<IUserRepository, SqlUserRepository>();
        services.AddScoped<DiplomacyAi>();

        services.AddSingleton(GameBalance.Buildings.GetAllTemplates);
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton(Options.Create(testJwtSettings));

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(
            typeof(RecruitUnitCommand).Assembly
        ));

        configure?.Invoke(services);

        var serviceProvider = services.BuildServiceProvider();

        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.EnsureCreated();

        return serviceProvider;
    }

    public void Dispose()
    {
        if (_dbName == null) return;

        SqliteConnection.ClearAllPools();

        if (File.Exists(_dbName))
        {
            File.Delete(_dbName);
        }

        GC.SuppressFinalize(this);
    }
}