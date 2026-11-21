using FluentAssertions;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using RTSCore.Application.Campaign.DisbandUnit;
using RTSCore.Infrastructure.Persistence;

namespace Tests.Application.Campaign.DisbandUnit;

[Collection("TestCollection")]
public sealed class DisbandUnitIntegrationTests(TestFixture testFixture) : IAsyncLifetime
{
    [Fact]
    public async Task Handle_ShouldDisbandUnit_AndSaveChangesToDb()
    {
        var army = TestData.CreateArmy();
        var unitId = army.AddReadyUnit(TestData.CreateUnitTemplate(maxHealth: 1)).Value;

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