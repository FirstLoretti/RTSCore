using FluentAssertions;

using RTSCore.Domain;
using RTSCore.Domain.AI.Diplomacy;
using RTSCore.Domain.Entities.Campaigns;
using RTSCore.Domain.Entities.Diplomacy;

namespace DomainUnitTests.AI.Diplomacy;

public class AiDiplomacyTests
{
    [Fact]
    public void ProcessDiplomacy_ShouldGenerateOutgoingProfitableOffers()
    {
        var relation1 = new DiplomacyRelation(CampaignId.Empty, FactionType.England, FactionType.France, DiplomacyRelation.MinStanding);
        var relation2 = new DiplomacyRelation(CampaignId.Empty, FactionType.England, FactionType.Spain, DiplomacyRelation.MaxStanding);
        var diplomacyContext = TestData.AiDiplomacyContext with { Relations = [relation1, relation2] };

        var decisions = AiDiplomacy.ProcessDiplomacy(diplomacyContext);

        decisions.TradeOfferToSend.Should().ContainSingle(x => x == FactionType.Spain);
        decisions.WarToDeclare.Should().ContainSingle(x => x == FactionType.France);
    }

    [Fact]
    public void ProcessDiplomacy_ShouldRespondToOffers()
    {
        var relation1 = new DiplomacyRelation(CampaignId.Empty, FactionType.England, FactionType.France, DiplomacyRelation.MinStanding);
        var relation2 = new DiplomacyRelation(CampaignId.Empty, FactionType.England, FactionType.Spain, DiplomacyRelation.MaxStanding);
        var diplomacyContext = TestData.AiDiplomacyContext with
        {
            Relations = [relation1, relation2],
            IncomingOffers = [
                new(CampaignId.Empty, FactionType.France, FactionType.England, OfferType.TradeAgreement),
                new(CampaignId.Empty, FactionType.Spain, FactionType.England, OfferType.TradeAgreement)
            ]
        };

        var decisions = AiDiplomacy.ProcessDiplomacy(diplomacyContext);

        decisions.OffersToReject.Should().ContainSingle(o => o.Initiator == FactionType.France);
        decisions.OffersToAccept.Should().ContainSingle(o => o.Initiator == FactionType.Spain);
    }
}