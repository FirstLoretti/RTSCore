using Microsoft.EntityFrameworkCore;

using RTSCore.Domain.Entities;
using RTSCore.Domain.ValueObjects;
using RTSCore.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using RTSCore.Domain.Services;

namespace RTSCore.Tests;

public class DatabaseIntegrationTests
{
    [Fact]
    public async Task UnitRepository_ShouldSaveAndLoadUnit()
    {
        const string dbName = "test.db";

        File.Delete(dbName);

        var options = CreateDb(dbName);

        var unit = CreateUnit();
        using (var context = new AppDbContext(options))
        {
            var repository = new SqlUnitRepository(context);
            var unitOfWork = new EfUnitOfWork(context);

            repository.Add(unit);

            await unitOfWork.SaveChangesAsync(CancellationToken.None);
        }

        Unit? dbUnit;
        using (var context = new AppDbContext(options))
        {
            var repository = new SqlUnitRepository(context);

            dbUnit = await repository.GetUnitAsync(unit.Id, CancellationToken.None);
        }

        SqliteConnection.ClearAllPools();
        File.Delete(dbName);

        Assert.NotNull(dbUnit);
        Assert.Equivalent(unit, dbUnit);
    }

    [Fact]
    public async Task UnitRepository_ShouldUpdateUnitStats_WhenLevelUp()
    {
        const string dbName = "test.db";

        File.Delete(dbName);

        var options = CreateDb(dbName);
        var unit = CreateUnit();

        using (var context = new AppDbContext(options))
        {
            var repository = new SqlUnitRepository(context);
            var unitOfWork = new EfUnitOfWork(context);

            repository.Add(unit);

            await unitOfWork.SaveChangesAsync(CancellationToken.None);
        }

        using (var context = new AppDbContext(options))
        {
            var repository = new SqlUnitRepository(context);
            var unitOfWork = new EfUnitOfWork(context);

            var dbUnit = await repository.GetUnitAsync(unit.Id, CancellationToken.None);

            if (dbUnit == null)
            {
                Assert.Fail("[Act] Юнит не найдет в базе данных.");
                return;
            }

            dbUnit.AddExperience(GameBalance.Units.ExpToNextLevel[0] + 1);

            await unitOfWork.SaveChangesAsync(CancellationToken.None);
        }

        Unit? updatedUnit;
        using (var context = new AppDbContext(options))
        {
            var repository = new SqlUnitRepository(context);

            updatedUnit = await repository.GetUnitAsync(unit.Id, CancellationToken.None);
        }

        SqliteConnection.ClearAllPools();
        File.Delete(dbName);

        Assert.NotNull(updatedUnit);
        Assert.True(updatedUnit.Level > unit.Level);
        Assert.True(updatedUnit.Experience > unit.Experience);
        Assert.True(
            updatedUnit.Health > unit.Health,
            $"Здоровье юнита из базы данных {updatedUnit.Health}, " +
            $"здоровье начального юнита {unit.Health}."
        );
    }

    private static Unit CreateUnit()
    {
        var unit = new Unit(
            id: "england_swordman_1",
            type: UnitType.EnglandSwordman,
            faction: FactionType.England
        );

        return unit;
    }

    private static DbContextOptions<AppDbContext> CreateDb(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite($"Data Source={dbName}")
            .Options;

        using var context = new AppDbContext(options);
        context.Database.EnsureCreated();

        return options;
    }
}