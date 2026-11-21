using FluentAssertions;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using RTSCore.Application.Campaign.ArmyCreation;
using RTSCore.Domain.Entities.Armies;
using RTSCore.Infrastructure.Persistence;

namespace Tests.Application.Campaign.ArmyCreation;

[Collection("TestCollection")]
public sealed class CreateArmyIntegrationTests(TestFixture testFixture) : IAsyncLifetime
{
    [Fact]
    public async Task Handle_ShouldCreateArmy_AndSaveToDb()
    {
        var city = TestData.CreateCity(template: TestData.CreateCityTemplate());

        using (var scope = testFixture.ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Cities.Add(city);
            await context.SaveChangesAsync();
        }

        ArmyId armyId;
        using (var scope = testFixture.ServiceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            armyId = await mediator.Send(new CreateArmyCommand(city.Id, UnitType.Peasant));
            armyId.Should().NotBe(ArmyId.Empty);
        }

        using (var scope = testFixture.ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            Army? army = await context.Armies
                .AsNoTracking()
                .Include(a => a.Units)
                .FirstAsync(a => a.Id == armyId);

            army.Units.Should().ContainSingle();
            army.Coordinates.Should().Be(city.Coordinates);
        }
    }

    public Task InitializeAsync() => testFixture.ResetDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;
}