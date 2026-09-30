using System.Numerics;

using FluentAssertions;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using NSubstitute;

using RTSCore.Application.Campaign.ArmyCreation;
using RTSCore.Domain.Entities;
using RTSCore.Domain.Exeptions;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.ValueObjects;
using RTSCore.Infrastructure.Persistence;

namespace RTSCore.Tests.Application.Campaign.ArmyCreation;

public class CreateArmyCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenRequestValid_ShouldCallServiceAndSaveChanges()
    {
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var service = Substitute.For<IArmyCreationService>();
        var city = City.CreateEmpty(CityType.Village, Vector2.Zero, FactionType.England);
        var army = Army.Create(FactionType.England, Vector2.Zero, new UnitTemplate());

        unitOfWork.CityRepository.GetAsync(city.Id, Arg.Any<CancellationToken>()).Returns(city);
        service.CreateArmy(city).Returns(army);

        var command = new CreateArmyCommand(city.Id);
        var handler = new CreateArmyCommandHandler(unitOfWork, service);

        await handler.Handle(command, CancellationToken.None);

        service.Received(1).CreateArmy(city);
        await unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }
}