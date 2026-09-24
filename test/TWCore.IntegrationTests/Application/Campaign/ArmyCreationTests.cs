using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using RTSCore.Application.Campaign.ArmyCreation;
using RTSCore.Domain.Entities;
using RTSCore.Domain.Exeptions;
using RTSCore.Domain.ValueObjects;
using RTSCore.Domain.ValueObjects.Presets;
using RTSCore.Infrastructure.Persistence;

namespace TWCore.IntegrationTests.Application.Campaign;

// public class CreateArmyCommandHandlerTests : TestBase
// {
//     private readonly UnitTemplate[] _unitTemplates = [new(
//         UnitType.Knight, "Unit", 1, 1, 1, 1, 1, 1, 1, 1, 1, UnitCategory.Infantry,1
//     )];
//     private readonly CityId _cityId = "id";

//     [Fact]
//     public async Task Handle_WhenValid_ShouldReturnArmyId()
//     {
//         var serviceProvider = SetupTestInvironment();

//         using (var scope = serviceProvider.CreateScope())
//         {
//             var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
//             var armyId = await mediator.Send(new CreateArmyCommand(_cityId));

//             Assert.NotEmpty(armyId);
//         }

//         using (var scope = serviceProvider.CreateScope())
//         {
//             var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//             var army = await context.Armies.FirstOrDefaultAsync();

//             Assert.NotNull(army);
//         }
//     }
// }