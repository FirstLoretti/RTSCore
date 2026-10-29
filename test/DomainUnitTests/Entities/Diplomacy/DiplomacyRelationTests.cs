using FluentAssertions;

using RTSCore.Domain;
using RTSCore.Domain.Entities.Campaigns;
using RTSCore.Domain.Entities.Diplomacy;
using RTSCore.Domain.Errors;

using static DomainUnitTests.TestData;

namespace DomainUnitTests.Entities.Diplomacy;

public class DiplomacyRelationTests
{
    public class Factory
    {
        [Fact]
        public void New_ShouldCreateWithCorrectFactionsOrder()
        {
            var relation1 = new DiplomacyRelation(CampaignId.Empty, FactionType.England, FactionType.France, DiplomacyConfig.InitialStanding);
            var relation2 = new DiplomacyRelation(CampaignId.Empty, FactionType.France, FactionType.England, DiplomacyConfig.InitialStanding);

            relation1.FactionA.Should().Be(relation2.FactionA);
        }
    }

    public class OpenTrade
    {
        [Fact]
        public void ShouldFailIfInWarState()
        {
            var relation = CreateDiplomacyRelation();
            relation.DeclareWar(DiplomacyConfig.DeclareWarPenalty);

            var result = relation.OpenTrade(DiplomacyConfig.AcceptTradeOfferBonus);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(GameErrors.CantTradeWithEnemy);
            relation.HasTradeAgreement.Should().BeFalse();
            relation.Standing.Should().Be(DiplomacyConfig.InitialStanding + DiplomacyConfig.DeclareWarPenalty);
        }

        [Fact]
        public void ShouldFailIfAlreadyTrade()
        {
            var relation = CreateDiplomacyRelation();
            relation.OpenTrade(DiplomacyConfig.AcceptTradeOfferBonus);

            var result = relation.OpenTrade(DiplomacyConfig.AcceptTradeOfferBonus);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(GameErrors.CantTradeAlreadyExist);
            relation.HasTradeAgreement.Should().BeTrue();
            relation.Standing.Should().Be(DiplomacyConfig.InitialStanding + DiplomacyConfig.AcceptTradeOfferBonus);
        }

        [Fact]
        public void ShouldSuccessIfAllConditionsMet()
        {
            var relation = CreateDiplomacyRelation();

            var result = relation.OpenTrade(DiplomacyConfig.AcceptTradeOfferBonus);

            result.IsSuccess.Should().BeTrue();
            relation.HasTradeAgreement.Should().BeTrue();
            relation.Standing.Should().Be(DiplomacyConfig.InitialStanding + DiplomacyConfig.AcceptTradeOfferBonus);
        }
    }

    public class DeclareWar
    {
        [Fact]
        public void ShouldFailIfAlreadyInWar()
        {
            var relation = CreateDiplomacyRelation();
            relation.DeclareWar(DiplomacyConfig.DeclareWarPenalty);

            var result = relation.DeclareWar(DiplomacyConfig.DeclareWarPenalty);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(GameErrors.CantWarFactionsAlreadyWar);
            relation.Standing.Should().Be(DiplomacyConfig.InitialStanding + DiplomacyConfig.DeclareWarPenalty);
            relation.InWar.Should().BeTrue();
        }

        [Fact]
        public void ShouldDeclareAndBreakTrade()
        {
            var relation = CreateDiplomacyRelation();
            relation.OpenTrade(DiplomacyConfig.AcceptTradeOfferBonus);

            var result = relation.DeclareWar(DiplomacyConfig.DeclareWarPenalty);

            result.IsSuccess.Should().BeTrue();
            relation.InWar.Should().BeTrue();
            relation.Standing.Should()
                .Be(DiplomacyConfig.InitialStanding + DiplomacyConfig.AcceptTradeOfferBonus + DiplomacyConfig.DeclareWarPenalty);
            relation.HasTradeAgreement.Should().BeFalse();
        }
    }

    public class MakePeace
    {
        [Fact]
        public void ShouldFailWhenNotAtWarState()
        {
            var relation = CreateDiplomacyRelation();

            var result = relation.MakePeace(DiplomacyConfig.AcceptPeaceOfferBonus);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(GameErrors.CantPeaceFactionsNotWar);
            relation.Standing.Should().Be(DiplomacyConfig.InitialStanding);
        }

        [Fact]
        public void ShouldSuccessWhenAllConditionsMet()
        {
            var relation = CreateDiplomacyRelation();
            relation.DeclareWar(DiplomacyConfig.DeclareWarPenalty);

            var result = relation.MakePeace(DiplomacyConfig.AcceptPeaceOfferBonus);

            result.IsSuccess.Should().BeTrue();
            relation.InWar.Should().BeFalse();
            relation.Standing.Should()
                .Be(DiplomacyConfig.InitialStanding + DiplomacyConfig.DeclareWarPenalty + DiplomacyConfig.AcceptPeaceOfferBonus);
        }
    }

    public class CancelTrade
    {
        [Fact]
        public void ShouldFailWhenTradeNotExist()
        {
            var relation = CreateDiplomacyRelation();

            var result = relation.CancelTrade(DiplomacyConfig.CancelTradePenalty);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(GameErrors.CantCancelTradeNotExist);
            relation.Standing.Should().Be(DiplomacyConfig.InitialStanding);
        }

        [Fact]
        public void ShouldCancelWhenAllConditionsMet()
        {
            var relation = CreateDiplomacyRelation();
            relation.OpenTrade(DiplomacyConfig.AcceptTradeOfferBonus);

            var result = relation.CancelTrade(DiplomacyConfig.CancelTradePenalty);

            result.IsSuccess.Should().BeTrue();
            relation.HasTradeAgreement.Should().BeFalse();
            relation.Standing.Should()
                .Be(DiplomacyConfig.InitialStanding + DiplomacyConfig.AcceptTradeOfferBonus + DiplomacyConfig.CancelTradePenalty);
        }
    }

    public class Standing
    {
        [Fact]
        public void Change_ShouldNotGoBelowMinStanding()
        {
            var relation = new DiplomacyRelation(CampaignId.Empty, FactionType.England, FactionType.France, DiplomacyRelation.MinStanding);

            relation.DeclareWar(DiplomacyConfig.DeclareWarPenalty);

            relation.Standing.Should().Be(DiplomacyRelation.MinStanding);
        }

        [Fact]
        public void Change_ShouldNotGoAboveMaxStanding()
        {
            var relation = new DiplomacyRelation(CampaignId.Empty, FactionType.England, FactionType.France, DiplomacyRelation.MaxStanding);

            relation.OpenTrade(DiplomacyConfig.AcceptTradeOfferBonus);

            relation.Standing.Should().Be(DiplomacyRelation.MaxStanding);
        }
    }
}