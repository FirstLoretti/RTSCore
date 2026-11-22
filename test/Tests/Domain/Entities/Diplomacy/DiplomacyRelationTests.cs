using FluentAssertions;

using RTSCore.Domain;
using RTSCore.Domain.Entities.Campaigns;
using RTSCore.Domain.Entities.Diplomacy;
using RTSCore.Domain.Errors;

using static Tests.TestData;

namespace Tests.Domain.Entities.Diplomacy;

public sealed class DiplomacyRelationTests
{
    public sealed class Factory
    {
        [Fact]
        public void New_ShouldCreateWithCorrectFactionsOrder()
        {
            var relation1 = new DiplomacyRelation(CampaignId.Empty, FactionType.England, FactionType.France, DiplomacyConfig.InitialStanding);
            var relation2 = new DiplomacyRelation(CampaignId.Empty, FactionType.France, FactionType.England, DiplomacyConfig.InitialStanding);

            relation1.FactionA.Should().Be(relation2.FactionA);
        }
    }

    public sealed class OpenTrade
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

    public sealed class MakePeace
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

    public sealed class CancelTrade
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

    public sealed class ChangeStanding
    {
        [Fact]
        public void ShouldNotGoBelowMinStanding()
        {
            var relation = new DiplomacyRelation(CampaignId.Empty, FactionType.England, FactionType.France, DiplomacyRelation.MinStanding);

            relation.DeclareWar(DiplomacyConfig.DeclareWarPenalty);

            relation.Standing.Should().Be(DiplomacyRelation.MinStanding);
        }

        [Fact]
        public void ShouldNotGoAboveMaxStanding()
        {
            var relation = new DiplomacyRelation(CampaignId.Empty, FactionType.England, FactionType.France, DiplomacyRelation.MaxStanding);

            relation.OpenTrade(DiplomacyConfig.AcceptTradeOfferBonus);

            relation.Standing.Should().Be(DiplomacyRelation.MaxStanding);
        }
    }

    public sealed class CreatePeaceOffer
    {
        [Theory]
        [InlineData(FactionType.England, FactionType.England)]
        [InlineData(FactionType.France, FactionType.England)]
        [InlineData(FactionType.England, FactionType.France)]
        public void ShouldReturnFailure_WhenInvalidFactions(FactionType initiator, FactionType target)
        {
            var relation = CreateDiplomacyRelation(factionA: FactionType.England, factionB: FactionType.Spain);

            var result = relation.CreatePeaceOffer(initiator, target);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(GameErrors.RelationNotContainFactions(initiator, target, relation.Id));
        }

        [Fact]
        public void ShouldReturnFailure_WhenNotInWar()
        {
            var relation = CreateDiplomacyRelation(factionA: FactionType.England, factionB: FactionType.Spain);

            var result = relation.CreatePeaceOffer(relation.FactionA, relation.FactionB);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(GameErrors.CantPeaceFactionsNotWar);
        }

        [Fact]
        public void ShouldReturnSuccess()
        {
            var relation = CreateDiplomacyRelation(factionA: FactionType.England, factionB: FactionType.Spain);
            relation.DeclareWar(default);

            var result = relation.CreatePeaceOffer(relation.FactionA, relation.FactionB);

            result.IsSuccess.Should().BeTrue();
            result.Value.Type.Should().Be(OfferType.PeaceTreaty);
            result.Value.Initiator.Should().Be(relation.FactionA);
            result.Value.Target.Should().Be(relation.FactionB);
        }
    }

    public sealed class CreateTradeOffer
    {
        [Theory]
        [InlineData(FactionType.England, FactionType.England)]
        [InlineData(FactionType.France, FactionType.England)]
        [InlineData(FactionType.England, FactionType.France)]
        public void ShouldReturnFailure_WhenInvalidFactions(FactionType initiator, FactionType target)
        {
            var relation = CreateDiplomacyRelation(factionA: FactionType.England, factionB: FactionType.Spain);

            var result = relation.CreateTradeOffer(initiator, target);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(GameErrors.RelationNotContainFactions(initiator, target, relation.Id));
        }

        [Fact]
        public void ShouldReturnFailure_WhenAlreadyTrade()
        {
            var relation = CreateDiplomacyRelation(factionA: FactionType.England, factionB: FactionType.Spain);
            relation.OpenTrade(default);

            var result = relation.CreateTradeOffer(relation.FactionA, relation.FactionB);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(GameErrors.CantTradeAlreadyExist);
        }

        [Fact]
        public void ShouldReturnFailure_WhenInWar()
        {
            var relation = CreateDiplomacyRelation(factionA: FactionType.England, factionB: FactionType.Spain);
            relation.DeclareWar(default);
            relation.OpenTrade(default);

            var result = relation.CreateTradeOffer(relation.FactionA, relation.FactionB);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(GameErrors.CantTradeWithEnemy);
        }

        [Fact]
        public void ShouldReturnSuccess()
        {
            var relation = CreateDiplomacyRelation(factionA: FactionType.England, factionB: FactionType.Spain);

            var result = relation.CreateTradeOffer(relation.FactionA, relation.FactionB);

            result.IsSuccess.Should().BeTrue();
            result.Value.Type.Should().Be(OfferType.TradeAgreement);
            result.Value.Initiator.Should().Be(relation.FactionA);
            result.Value.Target.Should().Be(relation.FactionB);
        }
    }
}