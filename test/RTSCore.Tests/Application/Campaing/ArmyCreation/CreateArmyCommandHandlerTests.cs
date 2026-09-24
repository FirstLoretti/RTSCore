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
using RTSCore.Domain.ValueObjects.Presets;
using RTSCore.Infrastructure.Persistence;

namespace RTSCore.Tests.Application.Campaing.ArmyCreation;

public class CreateArmyCommandHandlerTests
{
    private readonly UnitTemplate[] _unitTemplates = [new UnitTemplate()];

    [Fact]
    public async Task Handle_WhenRequestValid_ShouldCreateArmySaveToDbAndReturnArmyId()
    {
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var city = City.Create(
            CityType.Village,
            Vector2.Zero,
            FactionType.England,
            population: 1,
            _unitTemplates[0].Type
        );

        unitOfWork.CityRepository.GetCityAsync(city.Id, Arg.Any<CancellationToken>()).Returns(city);

        var command = new CreateArmyCommand(city.Id);
        var handler = new CreateArmyCommandHandler(unitOfWork, _unitTemplates);

        var armyId = await handler.Handle(command, CancellationToken.None);

        armyId.Should().NotBeNull();

        unitOfWork.ArmyRepository.Received(1).Add(Arg.Any<Army>());
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}