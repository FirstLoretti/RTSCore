using FluentAssertions;

using RTSCore.Domain;
using RTSCore.Domain.AI.Diplomacy;
using RTSCore.Domain.Entities.Diplomacy;

using static DomainUnitTests.TestData;

namespace DomainUnitTests.AI.Diplomacy;

public class AiOfferEvaluatorTests
{
    public class IsTradeOfferProfitable
    {
        [Fact]
        public void ShouldReturnFalse_IfAlreadyTrade()
        {
            var relation = CreateDiplomacyRelation();
            var initiator = AiFactionContext with { FactionType = relation.FactionA };
            var target = AiFactionContext with { FactionType = relation.FactionB, CitiesCount = 10 };
            relation.OpenTrade(standingBonus: 0);

            var result = AiOfferEvaluator.IsTradeOfferProfitable(relation, initiator, target);

            result.Should().BeFalse();
        }

        [Fact]
        public void ShouldReturnFalse_WhenInWarState()
        {
            var relation = CreateDiplomacyRelation();
            var initiator = AiFactionContext with { FactionType = relation.FactionA };
            var target = AiFactionContext with { FactionType = relation.FactionB, CitiesCount = 10 };
            relation.DeclareWar(standingPenalty: 0);

            var result = AiOfferEvaluator.IsTradeOfferProfitable(relation, initiator, target);

            result.Should().BeFalse();
        }

        [Fact]
        public void ShouldReturnTrue_WhenNotProfitable()
        {
            var relation = new DiplomacyRelation(FactionType.England, FactionType.France, -50);
            var initiator = AiFactionContext with { FactionType = relation.FactionA };
            var target = AiFactionContext with { FactionType = relation.FactionB };

            var result = AiOfferEvaluator.IsTradeOfferProfitable(relation, initiator, target);

            result.Should().BeFalse();
        }

        [Fact]
        public void ShouldReturnTrue_WhenProfitable()
        {
            var relation = CreateDiplomacyRelation();
            var initiator = AiFactionContext with { FactionType = relation.FactionA };
            var target = AiFactionContext with { FactionType = relation.FactionB, CitiesCount = 10 };

            var result = AiOfferEvaluator.IsTradeOfferProfitable(relation, initiator, target);

            result.Should().BeTrue();
        }
    }

    public class IsPeaceOfferProfitable
    {
        [Fact]
        public void ShouldReturnFalse_WhenNotInWarState()
        {
            var relation = CreateDiplomacyRelation();
            var initiator = AiFactionContext with { FactionType = relation.FactionA };
            var target = AiFactionContext with { FactionType = relation.FactionB, MilitaryPower = 100 };

            var result = AiOfferEvaluator.IsPeaceOfferProfitable(relation, initiator, target);

            result.Should().BeFalse();
        }

        [Fact]
        public void ShouldReturnFalse_WhenEnemyPowerZero()
        {
            var relation = CreateDiplomacyRelation();
            var initiator = AiFactionContext with { FactionType = relation.FactionA };
            var target = AiFactionContext with { FactionType = relation.FactionB, MilitaryPower = 0 };
            relation.DeclareWar(standingPenalty: 0);

            var result = AiOfferEvaluator.IsPeaceOfferProfitable(relation, initiator, target);

            result.Should().BeFalse();
        }

        [Fact]
        public void ShouldReturnFalse_WhenNotProfitable()
        {
            var relation = CreateDiplomacyRelation();
            var initiator = AiFactionContext with { FactionType = relation.FactionA, MilitaryPower = 100 };
            var target = AiFactionContext with { FactionType = relation.FactionB };
            relation.DeclareWar(standingPenalty: 0);

            var result = AiOfferEvaluator.IsPeaceOfferProfitable(relation, initiator, target);

            result.Should().BeFalse();
        }

        [Fact]
        public void ShouldReturnTrue_WhenProfitable()
        {
            var relation = CreateDiplomacyRelation();
            var initiator = AiFactionContext with { FactionType = relation.FactionA };
            var target = AiFactionContext with { FactionType = relation.FactionB, MilitaryPower = 100 };
            relation.DeclareWar(standingPenalty: 0);

            var result = AiOfferEvaluator.IsPeaceOfferProfitable(relation, initiator, target);

            result.Should().BeTrue();
        }
    }

    public class IsDeclareWarProfitable
    {
        [Fact]
        public void ShouldReturnFalse_WhenAlreadyInWarState()
        {
            var relation = CreateDiplomacyRelation();
            var initiator = AiFactionContext with { FactionType = relation.FactionA };
            var target = AiFactionContext with { FactionType = relation.FactionB };
            relation.DeclareWar(standingPenalty: 0);

            var result = AiOfferEvaluator.IsDeclareWarProfitable(relation, initiator, target);

            result.Should().BeFalse();
        }

        [Fact]
        public void ShouldReturnFalse_WhenNotProfitable()
        {
            var relation = CreateDiplomacyRelation();
            var initiator = AiFactionContext with { FactionType = relation.FactionA };
            var target = AiFactionContext with { FactionType = relation.FactionB, MilitaryPower = 100 };

            var result = AiOfferEvaluator.IsDeclareWarProfitable(relation, initiator, target);

            result.Should().BeFalse();
        }

        [Fact]
        public void ShouldReturnTrue_WhenOpponentHaveZeroPower()
        {
            var relation = CreateDiplomacyRelation();
            var initiator = AiFactionContext with { FactionType = relation.FactionA };
            var target = AiFactionContext with { FactionType = relation.FactionB, MilitaryPower = 0 };

            var result = AiOfferEvaluator.IsDeclareWarProfitable(relation, initiator, target);

            result.Should().BeTrue();
        }

        [Fact]
        public void ShouldReturnTrue_WhenProfitable()
        {
            var relation = CreateDiplomacyRelation();
            var initiator = AiFactionContext with { FactionType = relation.FactionA, MilitaryPower = 100 };
            var target = AiFactionContext with { FactionType = relation.FactionB };

            var result = AiOfferEvaluator.IsDeclareWarProfitable(relation, initiator, target);

            result.Should().BeTrue();
        }
    }
}