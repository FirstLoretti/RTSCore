using System.Numerics;

using FluentAssertions;

using RTSCore.Domain;
using RTSCore.Domain.AI;
using RTSCore.Domain.AI.Construction;
using RTSCore.Domain.AI.Personality;
using RTSCore.Domain.AI.Recruitment;
using RTSCore.Domain.Entities.Armies;
using RTSCore.Domain.Entities.Campaigns;
using RTSCore.Domain.Entities.Cities;
using RTSCore.Domain.Entities.Diplomacy;

using static Tests.TestData;

namespace Tests.Domain.AI;

public sealed class AiTurnOrchestratorTests
{
    [Fact]
    public void SimulateTurn_ShouldProcessIncomingOffersAndUpdateRelations()
    {
        var campaignId = CampaignId.New();
        var england = CreateFaction(campaignId, FactionTemplate with { Type = FactionType.England });
        var englandCity = CreateCity(campaignId, factionType: england.Type);
        var france = CreateFaction(campaignId, FactionTemplate with { Type = FactionType.France });
        var franceArmy = CreateArmy(campaignId, france.Type);
        var aiEngland = new AiFactionAgent(campaignId, FactionType.England, AiPersonalityType.Conqueror);
        var offer = CreateDiplomacyOffer(campaignId, france.Type, england.Type, OfferType.PeaceTreaty);
        var relation = CreateDiplomacyRelation(campaignId, england.Type, france.Type, standing: 100);
        relation.DeclareWar(standingPenalty: 0);

        var orchestrator = CreateOrchestrator();
        var worldState = new CampaignWorldState(
            factions: [england, france],
            cities: [englandCity],
            armies: [franceArmy],
            [relation],
            [offer],
            new UnitStatsCalculator(UnitConfiguration),
            UnitCatalog
        );

        orchestrator.SimulateTurn(aiEngland, worldState);

        offer.Status.Should().Be(OfferStatus.Accepted);
        relation.InWar.Should().BeFalse();
        worldState.CreatedOffers.Should().Contain(o => o.Type == OfferType.TradeAgreement);
    }

    [Fact]
    public void SimulateTurn_ShouldConstructBuildingsAndRecruitUnits()
    {
        var campaignId = CampaignId.New();
        var faction = CreateFaction(campaignId, FactionTemplate with { Type = FactionType.England });
        var startingGold = faction.Gold;
        var aiAgent = new AiFactionAgent(campaignId, faction.Type, AiPersonalityType.Conqueror);
        var city1 = CreateCity(campaignId, factionType: faction.Type, coordinates: Vector2.Zero);
        var city2 = CreateCity(campaignId, factionType: faction.Type, coordinates: Vector2.One);
        var army1 = CreateArmy(campaignId, factionType: faction.Type, coordinates: city1.Coordinates);
        var army2 = CreateArmy(campaignId, factionType: faction.Type, coordinates: city2.Coordinates);
        city1.ConstructInstantly(CreateBuildingTemplate() with { Type = BuildingType.MilitiaBarrack });

        var orchestrator = CreateOrchestrator();
        var worldState = new CampaignWorldState(
            factions: [faction],
            cities: [city1, city2],
            armies: [army1, army2],
            [],
            [],
            new UnitStatsCalculator(UnitConfiguration),
            UnitCatalog
        );

        orchestrator.SimulateTurn(aiAgent, worldState);

        army1.Units.Count.Should().BeGreaterThan(1);
        army2.Units.Should().ContainSingle();
        city1.Buildings.Count.Should().Be(2);
        city2.Buildings.Count.Should().Be(1);
        faction.Gold.Should().BeLessThan(startingGold);
    }

    private static AiTurnOrchestrator CreateOrchestrator() => new(
        TestData.AiCatalog,
        UnitCatalog,
        new AiRecruiter(UnitCatalog, TestData.AiCatalog, TestData.AiBudgetWeights),
        new AiConstructor(TestData.AiCatalog, BuildingCatalog, TestData.AiBudgetWeights),
        DiplomacyConfig
    );
}