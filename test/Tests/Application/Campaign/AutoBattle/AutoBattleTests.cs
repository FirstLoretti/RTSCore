using Ardalis.GuardClauses;

using FluentAssertions;

using NSubstitute;

using RTSCore.Application.Campaign.AutoBattle;
using RTSCore.Application.Repositories;
using RTSCore.Domain.Entities.Armies;
using RTSCore.Domain.Entities.Armies.Combat;

namespace Tests.Application.Campaign.AutoBattle;

public sealed class AutoBattleTests
{
    [Fact]
    public async Task Handle_ShouldThrow_WhenArmyNotFoundInDb()
    {
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var calculator = new AutoBattleCalculator(
            TestData.UnitCatalog,
            TestData.AutoBattleConfiguration,
            new UnitStatsCalculator(TestData.UnitConfiguration)
        );
        var handler = new AutoBattleCommandHandler(unitOfWork, calculator);

        var action = () => handler.Handle(new AutoBattleCommand(default, default), default);

        await action.Should().ThrowAsync<NotFoundException>();
        await unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }
}