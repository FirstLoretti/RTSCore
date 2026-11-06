using FluentAssertions;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using RTSCore.Application.Campaign.ArmyCreation;
using RTSCore.Domain.Entities.Armies;
using RTSCore.Infrastructure.Persistence;

namespace Tests.Application.Campaign;

[Collection("DbCollection")]
public sealed class CreateArmyTests(DbTestFixture dbFixture) : IAsyncLifetime
{
    [Fact]
    public async Task Handle_ShouldCreateArmyInCity_AndSaveToDb()
    {
        var city = TestData.CreateCity(TestData.CreateCityTemplate());

        using (var scope = dbFixture.ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Cities.Add(city);
            await context.SaveChangesAsync();
        }

        ArmyId armyId;
        using (var scope = dbFixture.ServiceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            armyId = await mediator.Send(new CreateArmyCommand(city.Id, UnitType.Peasant));
            armyId.Should().NotBe(ArmyId.Empty);
        }

        using (var scope = dbFixture.ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            Army? army = await context.Armies
                .AsNoTracking()
                .Include(a => a.Units)
                .FirstOrDefaultAsync(a => a.Id == armyId);

            army.Should().NotBeNull();
            army.Units.Should().ContainSingle();
            army.Coordinates.Should().Be(city.Coordinates);
        }
    }

    public Task InitializeAsync() => dbFixture.ResetDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;
}