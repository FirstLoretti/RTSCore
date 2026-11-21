using System.Numerics;

using FluentAssertions;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using RTSCore.Application.Campaign.ArmyMovement;
using RTSCore.Infrastructure.Persistence;

namespace Tests.Application.Campaign.ArmyMovement;

[Collection("TestCollection")]
public sealed class MoveArmyIntegrationTests(TestFixture testFixture) : IAsyncLifetime
{
    [Fact]
    public async Task Handle_ShouldMoveToDestination_AndSaveToDb()
    {
        var army = TestData.CreateArmy();
        var destination = new Vector2(5f, 5f);
        army.RestoreMovementPoints(TestData.MovementConfiguration);

        using (var scope = testFixture.ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Armies.Add(army);
            await context.SaveChangesAsync();
        }

        using (var scope = testFixture.ServiceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var result = await mediator.Send(new MoveArmyCommand(army.Id, destination.X, destination.Y));

            result.IsSuccess.Should().BeTrue();
        }

        using (var scope = testFixture.ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var dbArmy = await context.Armies.AsNoTracking().FirstAsync(a => a.Id == army.Id);

            dbArmy.MovementPoints.Should().BeLessThan(army.MovementPoints);
            dbArmy.Coordinates.Should().Be(destination);
        }
    }

    public Task InitializeAsync() => testFixture.ResetDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;
}