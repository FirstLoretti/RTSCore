using FluentAssertions;

using RTSCore.Domain.Entities.Diplomacy;
using RTSCore.Domain.Errors;

using static Tests.TestData;

namespace Tests.Domain.Entities.Diplomacy;

public sealed class DiplomacyOfferTests
{
    [Fact]
    public void AcceptOffer_ShouldReturnFailure_WhenOfferNotInPendingStatus()
    {
        var offer = CreateDiplomacyOffer();
        offer.Reject();

        var result = offer.Accept();

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(GameErrors.InvalidOfferStatus);
        offer.Status.Should().Be(OfferStatus.Rejeсted);
    }

    [Fact]
    public void AcceptOffer_ShouldReturnSuccess()
    {
        var offer = CreateDiplomacyOffer();

        var result = offer.Accept();

        result.IsSuccess.Should().BeTrue();
        offer.Status.Should().Be(OfferStatus.Accepted);
    }

    [Fact]
    public void RejectOffer_ShouldReturnFailure_WhenOfferNotInPendingStatus()
    {
        var offer = CreateDiplomacyOffer();
        offer.Accept();

        var result = offer.Reject();

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(GameErrors.InvalidOfferStatus);
        offer.Status.Should().Be(OfferStatus.Accepted);
    }

    [Fact]
    public void RejectOffer_ShouldReturnSuccess()
    {
        var offer = CreateDiplomacyOffer();

        var result = offer.Reject();

        result.IsSuccess.Should().BeTrue();
        offer.Status.Should().Be(OfferStatus.Rejeсted);
    }
}