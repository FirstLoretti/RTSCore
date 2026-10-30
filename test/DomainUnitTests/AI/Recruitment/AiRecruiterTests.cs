using FluentAssertions;

using RTSCore.Domain.AI.Personality;
using RTSCore.Domain.AI.Recruitment;
using RTSCore.Domain.Entities.Armies;

using static DomainUnitTests.TestData;

namespace DomainUnitTests.AI.Recruitment;

public class AiRecruiterTests
{

    [Fact]
    public void New_ShouldEvaluateUnitsAndOrder()
    {
        var aiRecruiter = new AiRecruiter(UnitCatalog, TestData.AiCatalog);

        aiRecruiter.GetUnits(AiPersonalityType.Conqueror).First().Type.Should().Be(UnitType.Peasant);
        aiRecruiter.GetUnits(AiPersonalityType.Defender).First().Type.Should().Be(UnitType.PeasantArcher);
    }

    [Fact]
    public void RecruitUnit_ShouldEvaulateAndRecruitUsefulest()
    {
        var faction = CreateFaction();
        var army = CreateArmy();
        var unit1 = CreateProductionOption(UnitType.PeasantArcher) with { Cost = 2 };
        var unit2 = CreateProductionOption(UnitType.Peasant) with { Cost = (int)((faction.Gold - 1) / 2f) };
        var aiRecruiter = new AiRecruiter(UnitCatalog, TestData.AiCatalog);

        aiRecruiter.RecruitUnits(faction, army, AiPersonalityType.Conqueror, [unit1, unit2]);

        army.Units.Count.Should().Be(4);
        army.Units[1].Type.Should().Be(UnitType.Peasant);
        army.Units[2].Type.Should().Be(UnitType.Peasant);
        army.Units[3].Type.Should().Be(UnitType.PeasantArcher);
        faction.Gold.Should().Be(0);
    }
}