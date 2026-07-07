using Microsoft.EntityFrameworkCore;

using RTSCore.Domain.Entities;
using RTSCore.Domain.ValueObjects;
using RTSCore.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;

namespace RTSCore.Tests;

public class DatabaseIntegrationTests
{
    [Fact]
    public void UnitRepository_ShouldSaveAndLoadUnit()
    {
        const string dbName = "test.db";

        if (File.Exists(dbName)) File.Delete(dbName);

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite($"Data Source={dbName}")
            .Options;

        using (var context = new AppDbContext(options))
        {
            context.Database.EnsureCreated();
        }

        var unitTemplate = new UnitTemplate(
            Type: UnitType.EnglandSwordman,
            DisplayName: "EnglandSwordman",
            MaxHealth: 100,
            Damage: 25,
            Armor: 2,
            Speed: 5,
            ExpKillReward: 50,
            HealthGrowthRate: 1.1f,
            DamageGrowthRate: 1.15f
        );

        var unit = new Unit(
            id: "england_swordman_1",
            type: unitTemplate.Type,
            template: unitTemplate,
            faction: FactionType.England
        );

        using (var context = new AppDbContext(options))
        {
            var repository = new SqlUnitRepository(context);
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
}