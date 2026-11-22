using FluentAssertions;

using RTSCore.Domain.AI.Personality;
using RTSCore.Domain.AI.Recruitment;
using RTSCore.Domain.Configurations.Armies;
using RTSCore.Domain.Entities.Armies;

namespace Tests.Domain.AI.Recruitment;

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
        var faction = TestData.CreateFaction();
        var army = TestData.CreateArmy();
        var unit1 = TestData.CreateProductionOption(UnitType.PeasantArcher) with { Cost = 2 };
        var unit2 = TestData.CreateProductionOption(UnitType.Peasant) with { Cost = (int)((faction.Gold - 1) / 2f) };
        var aiRecruiter = new AiRecruiter(UnitCatalog, TestData.AiCatalog);

        aiRecruiter.RecruitUnits(faction, army, AiPersonalityType.Conqueror, [unit1, unit2]);

        army.Units.Count.Should().Be(4);
        army.Units[1].Type.Should().Be(UnitType.Peasant);
        army.Units[2].Type.Should().Be(UnitType.Peasant);
        army.Units[3].Type.Should().Be(UnitType.PeasantArcher);
        faction.Gold.Should().Be(0);
    }

    [Fact]
    public void RecruitUnit_ShouldNotRecruit_WhenNotEnoughGold()
    {
        var faction = TestData.CreateFaction();
        var initialGold = faction.Gold;
        var army = TestData.CreateArmy();
        var option = TestData.CreateProductionOption(UnitType.Peasant) with { Cost = faction.Gold + 1 };
        var aiRecruiter = new AiRecruiter(UnitCatalog, TestData.AiCatalog);

        aiRecruiter.RecruitUnits(faction, army, AiPersonalityType.Conqueror, [option]);

        army.Units.Should().ContainSingle();
        faction.Gold.Should().Be(initialGold);
    }

    [Fact]
    public void RecruitUnit_ShouldNotRecruit_WhenArmyIsFull()
    {
        var faction = TestData.CreateFaction();
        var initialGold = faction.Gold;
        var army = TestData.CreateArmy();
        var option = TestData.CreateProductionOption(UnitType.Peasant) with { Cost = faction.Gold };
        var aiRecruiter = new AiRecruiter(UnitCatalog, TestData.AiCatalog);
        FillArmy(army);

        aiRecruiter.RecruitUnits(faction, army, AiPersonalityType.Conqueror, [option]);

        army.Units.Count.Should().Be(Army.MaxSize);
        faction.Gold.Should().Be(initialGold);
    }

    private UnitCatalog UnitCatalog { get; } = new(
        [
           TestData.CreateUnitTemplate(maxHealth: 1) with
            {
                Type = UnitType.Peasant,
                Category = UnitCategory.Infantry
            },
            TestData.CreateUnitTemplate(maxHealth: 1) with
            {
                Type = UnitType.PeasantArcher,
                Category = UnitCategory.RangeInfantry
            }
        ]
    );

    private static void FillArmy(Army army)
    {
        for (int i = 0; i < Army.MaxSize; i++)
        {
            army.RecruitUnit(TestData.CreateUnitTemplate(maxHealth: 1));
        }
    }
}