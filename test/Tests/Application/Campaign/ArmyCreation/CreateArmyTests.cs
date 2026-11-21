using Ardalis.GuardClauses;

using FluentAssertions;

using NSubstitute;

using RTSCore.Application.Campaign.ArmyCreation;
using RTSCore.Application.Repositories;

namespace Tests.Application.Campaign.ArmyCreation;

public sealed class CreateArmyTests
{
    [Fact]
    public async Task Handle_ShouldThrow_WhenCityDoesNotExistInDb()
    {
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var handler = new CreateArmyCommandHandler(unitOfWork, TestData.UnitCatalog);

        var action = () => handler.Handle(new CreateArmyCommand(default, default), default);

        await action.Should().ThrowAsync<NotFoundException>();
        await unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }
}