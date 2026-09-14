using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using RTSCore.Application.Army.Commands;
using RTSCore.Domain.Entities;
using RTSCore.Domain.Exeptions;
using RTSCore.Domain.ValueObjects;
using RTSCore.Domain.ValueObjects.Presets;
using RTSCore.Infrastructure.Persistence;
using RTSCore.Tests.Base;

namespace RTSCore.Tests.Application.Armies;

public class CreateArmyCommandHandlerTests : TestBase
{
    private readonly UnitTemplate[] _unitTemplates = [new(UnitType.Knight, "Unit", 1, 1, 1, 1, 1, 1, 1, 1, 1)];
    private readonly CityId _cityId = "id";

    [Fact]
    public async Task Handle_WhenValid_ShouldReturnArmyId()
    {
        var serviceProvider = await ArrangeEnvironment(_unitTemplates);

        using (var scope = serviceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var armyId = await mediator.Send(new CreateArmyCommand(_cityId));

            Assert.NotEmpty(armyId);
        }

        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var army = await context.Armies.FirstOrDefaultAsync();

            Assert.NotNull(army);
        }
    }

    [Fact]
    public async Task Handle_WhenCityNotFound_ShouldThrow()
    {
        var serviceProvider = await ArrangeEnvironment(_unitTemplates);

        using (var scope = serviceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var command = new CreateArmyCommand("invalid_id");

            await Assert.ThrowsAsync<NotFoundException>(async () => await mediator.Send(command));
        }

        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var army = await context.Armies.SingleOrDefaultAsync();

            Assert.Null(army);
        }
    }

    [Fact]
    public async Task Handle_WhenTemplateNotFound_ShouldThrow()
    {
        var serviceProvider = await ArrangeEnvironment([]);

        using var scope = serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var command = new CreateArmyCommand(_cityId);

        await Assert.ThrowsAsync<NotFoundException>(async () => await mediator.Send(command));

        var army = await context.Armies.SingleOrDefaultAsync();
        Assert.Null(army);
    }

    private async Task<ServiceProvider> ArrangeEnvironment(UnitTemplate[] unitTemplates)
    {
        var cityPreset = new CityPreset(_cityId, "City", CityType.Village, 1, []);
        var city = new City(cityPreset, FactionType.England, new Coordinates(5, 5));

        var serviceProvider = SetupTestInvironment(options =>
            options.AddSingleton<IReadOnlyCollection<UnitTemplate>>(unitTemplates)
        );

        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            context.Cities.Add(city);
            await context.SaveChangesAsync();
        }

        return serviceProvider;
    }
}