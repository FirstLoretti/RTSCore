using FluentAssertions;

using RTSCore.Domain;
using RTSCore.Domain.Entities.Campaigns;

namespace Tests.Domain.Entities.Campaigns;

public class WorldInitializerTests
{
    [Fact]
    public void Initialize_ShouldReturnInitializeWorldResult()
    {
        HashSet<FactionType> playersFactions = [FactionType.England, FactionType.France];

        var result = WorldInitializer.Initialize(CampaignId.New(), playersFactions, TestData.FactionCatalog, TestData.CityCatalog);

        result.Campaign.Id.Should().NotBe(CampaignId.Empty);
        result.Cities.Count.Should().Be(TestData.FactionCatalog.AllTemplates.SelectMany(f => f.CityPresets).Count());
        result.Factions.Count.Should().Be(TestData.FactionCatalog.AllTemplates.Length);

        result.AiAgents.Count.Should().Be(TestData.FactionCatalog.AllTemplates.Length - playersFactions.Count);
        result.AiAgents.Select(a => a.Faction).Should().Contain(FactionType.Spain);
        result.AiAgents.Select(a => a.Faction).Should().NotContain(playersFactions);
    }
}