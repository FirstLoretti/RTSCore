using Ardalis.GuardClauses;

using FluentAssertions;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using RTSCore.Application.Campaign.DisbandUnit;
using RTSCore.Domain.Entities.Armies;
using RTSCore.Domain.Errors;
using RTSCore.Infrastructure.Persistence;

namespace Tests.Application.Campaign;

[Collection("TestCollection")]
public sealed class DisbandUnitTests(TestFixture fixture) : IAsyncLifetime
{
    [Fact]
    public async Task Handle_ShouldThrow_WhenArmyDoesNotExistInDb()
    {
        using var scope = fixture.ServiceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var action = () => mediator.Send(new DisbandUnitCommand(ArmyId.New(), UnitId.New()));

        await action.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_ShouldFailure_WhenUnitDoesNotExistInArmy()
    {
        var army = TestData.CreateArmy();

        using (var scope = fixture.ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Armies.Add(army);
            await context.SaveChangesAsync();
        }

        using (var scope = fixture.ServiceProvider.CreateScope())
        {
            var unitId = UnitId.New();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var result = await mediator.Send(new DisbandUnitCommand(army.Id, unitId));

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(GameErrors.CantDisbandUnitNotInThisArmy(unitId, army.Id));
        }
    }

    [Fact]
    public async Task Handle_ShouldDisbandUnit_AndSaveChangesToDb()
    {
        var army = TestData.CreateArmy();
        var unitId = army.AddReadyUnit(TestData.CreateUnitTemplate(maxHealth: 100)).Value;

        using (var scope = fixture.ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Armies.Add(army);
            await context.SaveChangesAsync();
        }

        using (var scope = fixture.ServiceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var result = await mediator.Send(new DisbandUnitCommand(army.Id, unitId));

            result.IsSuccess.Should().BeTrue();
        }

        using (var scope = fixture.ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var dbArmy = await context.Armies
                .AsNoTracking()
                .Include(a => a.Units)
                .FirstAsync(a => a.Id == army.Id);

            dbArmy.Units.Should().NotContain(u => u.Id == unitId);
        }
    }

    public Task InitializeAsync() => fixture.ResetDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;
}