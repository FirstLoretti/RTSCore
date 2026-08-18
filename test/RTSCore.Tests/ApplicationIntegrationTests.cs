using MediatR;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using RTSCore.Application.Common.Behaviors;
using RTSCore.Application.Units.Commands;
using RTSCore.Application.Units.Queries;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.ValueObjects;
using RTSCore.Infrastructure.Persistence;

using Unit = RTSCore.Domain.Entities.Unit;

namespace RTSCore.Tests;

public class ApplicationIntegrationTests
{
    [Fact]
    public async Task Mediator_ShouldRouteCreateAndGetUnitCommandToHandlers_AndPassThroughLoggingBehavior()
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
    public async Task Mediator_ShouldRouteAddExperienceCommandToHandler_PassThroughLoggingBehaviorAndChageLvlAndExp()
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

    private static (string, ServiceProvider) Arrange()
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
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(
            typeof(CreateUnitCommand).Assembly
        ));

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
}