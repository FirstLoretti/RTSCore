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
    public void UnitRepository_ShouldSaveAndLoadUnit()
    {
        const string dbName = "test.db";

        if (File.Exists(dbName)) File.Delete(dbName);

        var options = CreateDb(dbName);

        var unit = CreateUnit();
        using (var context = new AppDbContext(options))
        {
            var repository = new SqlUnitRepository(context);
            repository.Add(unit);
            repository.Save(unit);
        }

        Unit? dbUnit;
        using (var context = new AppDbContext(options))
        {
            var repository = new SqlUnitRepository(context);
            dbUnit = repository.GetUnit(unit.Id);
        }

        SqliteConnection.ClearAllPools();
        if (File.Exists(dbName)) File.Delete(dbName);

        Assert.NotNull(dbUnit);
        Assert.Equivalent(unit, dbUnit);
    }

    [Fact]
    public void UnitRepository_ShouldUpdateUnitStats_WhenLevelUp()
    {
        const string dbName = "test.db";

        if (File.Exists(dbName)) File.Delete(dbName);

        var options = CreateDb(dbName);
        var unit = CreateUnit();

        using (var context = new AppDbContext(options))
        {
            var repository = new SqlUnitRepository(context);
            repository.Add(unit);
            repository.Save(unit);
        }

        using (var context = new AppDbContext(options))
        {
            var repository = new SqlUnitRepository(context);

            if (repository.GetUnit(unit.Id) is not Unit dbUnit)
            {
                Assert.Fail("[Act] Юнит не найдет в базе данных.");
                return;
            }

            dbUnit.AddExperience(GameBalance.Units.ExpToNextLevel[0] + 1);
            repository.Save(dbUnit);
        }

        Unit? updatedUnit;
        using (var context = new AppDbContext(options))
        {
            var repository = new SqlUnitRepository(context);
            updatedUnit = repository.GetUnit(unit.Id);
        }

        SqliteConnection.ClearAllPools();
        if (File.Exists(dbName)) File.Delete(dbName);

        Assert.NotNull(updatedUnit);
        Assert.True(updatedUnit.Level > unit.Level);
        Assert.True(updatedUnit.Experience > unit.Experience);
        Assert.True(
            updatedUnit.Health > unit.Health,
            $"Здоровье юнита из базы данных {updatedUnit.Health}, здоровье начального юнита {unit.Health}."
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