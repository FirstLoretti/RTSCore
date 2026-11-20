using FluentAssertions;

using RTSCore.Domain.Errors;

using static Tests.TestData;

namespace Tests.Domain.Entities.Factions;

public sealed class FactionTests
{
    [Fact]
    public void SpendGold_ShouldReturnFailure_WhenGoldDoesNotEnough()
    {
        var faction = CreateFaction();
        var startingGold = faction.Gold;

        var result = faction.SpendGold(faction.Gold + 1);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(GameErrors.NotEnoughGold);
        faction.Gold.Should().Be(startingGold);
    }

    [Fact]
    public void SpendGold_ShouldReturnSuccess()
    {
        var faction = CreateFaction();

        var result = faction.SpendGold(faction.Gold);

        result.IsSuccess.Should().BeTrue();
        faction.Gold.Should().Be(0);
    }
}