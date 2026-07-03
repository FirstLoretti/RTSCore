using Microsoft.EntityFrameworkCore;

using RTSCore.Domain.Entities;
using RTSCore.Domain.ValueObjects;
using RTSCore.Infrastructure.Persistence;

namespace RTSCore.Tests;

public class DatabaseIntegrationTests
{
    [Fact]
    public void DbContext_ShouldSaveLoadUnit()
    {
        const string testDbName = "test_game.db";

        if (File.Exists(testDbName)) File.Delete(testDbName);

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite($"Data Source={testDbName}")
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
            context.Units.Add(unit);
            context.SaveChanges();
        }

        using (var context = new AppDbContext(options))
        {
            var dbUnit = context.Units.Find(new UnitId("england_swordman_1"));

            Assert.NotNull(dbUnit);
            Assert.Equal(dbUnit.Id, unit.Id);
            Assert.Equal(dbUnit.Type, unit.Type);
            Assert.Equal(dbUnit.Faction, unit.Faction);
            Assert.Equal(dbUnit.Health, unit.Health);
            Assert.Equal(dbUnit.Damage, unit.Damage);
            Assert.Equal(dbUnit.Armor, unit.Armor);
            Assert.Equal(dbUnit.Level, unit.Level);
            Assert.Equal(dbUnit.Experience, unit.Experience);
        }

        Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
        if (File.Exists(testDbName)) File.Delete(testDbName);
    }
}