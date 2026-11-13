using Ardalis.GuardClauses;

using FluentAssertions;

using NSubstitute;

using RTSCore.Application.Campaign.Diplomacy.OfferResponses;
using RTSCore.Application.Repositories;
using RTSCore.Domain;
using RTSCore.Domain.Entities.Campaigns;
using RTSCore.Domain.Entities.Diplomacy;
using RTSCore.Domain.Errors;

using static Tests.TestData;

namespace Tests.Application.Campaign.Diplomacy.OfferResponses;

public sealed class AcceptOfferHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private AcceptOfferCommandHandler CreateHandler() => new(_unitOfWork, DiplomacyConfig);

    [Fact]
    public async Task Handle_ShouldThrowAndNotSaveToDb_WhenOfferNotExist()
    {
        var command = new AcceptOfferCommand(DiplomacyOfferId.New());

        var action = () => CreateHandler().Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>();
        await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_ShouldThrowAndNotSaveToDb_WhenRelationNotExist()
    {
        var command = new AcceptOfferCommand(DiplomacyOfferId.New());
        _unitOfWork.DiplomacyOfferRepository.GetAsync(default, default).ReturnsForAnyArgs(CreateDiplomacyOffer());

        var action = () => CreateHandler().Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>();
        await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_ShouldThrowAndNotSaveToDb_WhenOfferTypeNotExist()
    {
        var command = new AcceptOfferCommand(DiplomacyOfferId.New());
        _unitOfWork.DiplomacyOfferRepository.GetAsync(default, default).ReturnsForAnyArgs(CreateDiplomacyOffer(offerType: (OfferType)99));
        _unitOfWork.DiplomacyRelationRepository.GetAsync(default, default, default, default).ReturnsForAnyArgs(CreateDiplomacyRelation());

        var action = () => CreateHandler().Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<InvalidOperationException>();
        await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailureAndNotSaveToDb_WhenAlreadyTrade()
    {
        var relation = CreateDiplomacyRelation(CampaignId.New(), FactionType.England, FactionType.France);
        var offer = CreateDiplomacyOffer(relation.CampaignId, FactionType.England, FactionType.France, OfferType.TradeAgreement);
        var command = new AcceptOfferCommand(offer.Id);
        relation.OpenTrade(standingBonus: 0);

        _unitOfWork.DiplomacyOfferRepository.GetAsync(default, default).ReturnsForAnyArgs(offer);
        _unitOfWork.DiplomacyRelationRepository.GetAsync(default, default, default, default).ReturnsForAnyArgs(relation);

        var result = await CreateHandler().Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(GameErrors.CantTradeAlreadyExist);
        await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailureAndNotSaveToDb_WhenNotInWar()
    {
        var relation = CreateDiplomacyRelation(CampaignId.New(), FactionType.England, FactionType.France);
        var offer = CreateDiplomacyOffer(relation.CampaignId, FactionType.England, FactionType.France, OfferType.PeaceTreaty);
        var command = new AcceptOfferCommand(offer.Id);

        _unitOfWork.DiplomacyOfferRepository.GetAsync(default, default).ReturnsForAnyArgs(offer);
        _unitOfWork.DiplomacyRelationRepository.GetAsync(default, default, default, default).ReturnsForAnyArgs(relation);

        var result = await CreateHandler().Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(GameErrors.CantPeaceFactionsNotWar);
        await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }
}