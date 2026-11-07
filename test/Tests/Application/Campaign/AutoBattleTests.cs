using Ardalis.GuardClauses;

using FluentAssertions;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using RTSCore.Application.Campaign.AutoBattle;
using RTSCore.Domain.Entities.Armies;
using RTSCore.Domain.Entities.Armies.Combat;
using RTSCore.Infrastructure.Persistence;

namespace Tests.Application.Campaign;

[Collection("TestCollection")]
public sealed class AutoBattleTests(TestFixture fixture) : IAsyncLifetime
{
    [Fact]
    public async Task Handle_ShouldThrow_WhenArmyNotFoundInDb()
    {
        using var scope = fixture.ServiceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var action = () => mediator.Send(new AutoBattleCommand(ArmyId.New(), ArmyId.New()));

        await action.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_ShouldCalculate_ApplyCasualties_SaveToDb_AndReturnResult()
    {
        var attacker = TestData.CreateArmy();
        var defender = TestData.CreateArmy();
        var unit = TestData.CreateUnitTemplate(maxHealth: 100);
        attacker.AddReadyUnit(unit);

        using (var scope = fixture.ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Armies.Add(attacker);
            context.Armies.Add(defender);

            await context.SaveChangesAsync();
        }

        BattleResult battleResult = default;
        using (var scope = fixture.ServiceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            battleResult = await mediator.Send(new AutoBattleCommand(attacker.Id, defender.Id));
        }

        using (var scope = fixture.ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var dbAttacker = context.Armies
                .AsNoTracking()
                .Include(a => a.Units)
                .First(a => a.Id == attacker.Id);
            var dbDefender = context.Armies
                .AsNoTracking()
                .Include(a => a.Units)
                .First(a => a.Id == defender.Id);

            dbAttacker.Units[1].Health.Should().BeLessThan(unit.MaxHealth);
            dbDefender.Units.Should().BeEmpty();
        }
    }

    public Task InitializeAsync() => fixture.ResetDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;
}