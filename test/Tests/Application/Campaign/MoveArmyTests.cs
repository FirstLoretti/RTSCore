using System.Numerics;

using Ardalis.GuardClauses;

using FluentAssertions;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using RTSCore.Application.Campaign.ArmyMovement;
using RTSCore.Domain.Entities.Armies;
using RTSCore.Domain.Errors;
using RTSCore.Infrastructure.Persistence;

namespace Tests.Application.Campaign;

[Collection("TestCollection")]
public sealed class MoveArmyTests(TestFixture fixture) : IAsyncLifetime
{
    [Fact]
    public async Task Handle_ShouldThrow_WhenArmyNotFoundInDb()
    {
        using var scope = fixture.ServiceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var action = () => mediator.Send(new MoveArmyCommand(ArmyId.New(), 0f, 0f));

        await action.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenArmyMovementPointsDoesNotEnough()
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
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var result = await mediator.Send(new MoveArmyCommand(army.Id, 100f, 100f));

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(GameErrors.NotEnoughMovementPoints);
        }

        using (var scope = fixture.ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var dbArmy = await context.Armies
                .AsNoTracking()
                .FirstAsync(a => a.Id == army.Id);

            dbArmy.Coordinates.Should().Be(army.Coordinates);
        }
    }

    [Fact]
    public async Task Handle_ShouldMoveToDestination_AndSaveToDb()
    {
        var army = TestData.CreateArmy();
        var destination = new Vector2(5f, 5f);
        army.RestoreMovementPoints(TestData.MovementConfiguration);

        using (var scope = fixture.ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Armies.Add(army);
            await context.SaveChangesAsync();
        }

        using (var scope = fixture.ServiceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var result = await mediator.Send(new MoveArmyCommand(army.Id, destination.X, destination.Y));

            result.IsSuccess.Should().BeTrue();
        }

        using (var scope = fixture.ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var dbArmy = await context.Armies.AsNoTracking().FirstAsync(a => a.Id == army.Id);

            dbArmy.MovementPoints.Should().BeLessThan(army.MovementPoints);
            dbArmy.Coordinates.Should().Be(destination);
        }
    }

    public Task InitializeAsync() => fixture.ResetDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;
}