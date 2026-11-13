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
public sealed class DisbandUnitTests(TestFixture testFixture) : IAsyncLifetime
{
    [Fact]
    public async Task Handle_ShouldThrow_WhenArmyDoesNotExistInDb()
    {
        using var scope = testFixture.ServiceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var action = () => mediator.Send(new DisbandUnitCommand(ArmyId.New(), UnitId.New()));

        await action.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_ShouldFailure_WhenUnitDoesNotExistInArmy()
    {
        var army = TestData.CreateArmy();

        using (var scope = testFixture.ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Armies.Add(army);
            await context.SaveChangesAsync();
        }

        using (var scope = testFixture.ServiceProvider.CreateScope())
        {
            var unitId = UnitId.New();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var result = await mediator.Send(new DisbandUnitCommand(army.Id, unitId));

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(GameErrors.UnitNotInTargetArmy(unitId, army.Id));
        }
    }

    [Fact]
    public async Task Handle_ShouldDisbandUnit_AndSaveChangesToDb()
    {
        var army = TestData.CreateArmy();
        var unitId = army.AddReadyUnit(TestData.CreateUnitTemplate(maxHealth: 100)).Value;

        using (var scope = testFixture.ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Armies.Add(army);
            await context.SaveChangesAsync();
        }

        using (var scope = testFixture.ServiceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var result = await mediator.Send(new DisbandUnitCommand(army.Id, unitId));

            result.IsSuccess.Should().BeTrue();
        }

        using (var scope = testFixture.ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var dbArmy = await context.Armies
                .AsNoTracking()
                .Include(a => a.Units)
                .FirstAsync(a => a.Id == army.Id);

            dbArmy.Units.Should().NotContain(u => u.Id == unitId);
        }
    }

    public Task InitializeAsync() => testFixture.ResetDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;
}