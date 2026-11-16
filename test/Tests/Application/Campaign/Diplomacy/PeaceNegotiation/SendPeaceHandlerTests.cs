using Ardalis.GuardClauses;

using FluentAssertions;

using NSubstitute;

using RTSCore.Application.Campaign.Diplomacy.PeaceNegotiation;
using RTSCore.Application.Repositories;
using RTSCore.Domain;
using RTSCore.Domain.Entities.Campaigns;
using RTSCore.Domain.Errors;

namespace Tests.Application.Campaign.Diplomacy.PeaceNegotiation;

public sealed class SendPeaceHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private SendPeaceOfferCommandHandler CreateHandler() => new(_unitOfWork);

    [Fact]
    public async Task Handle_ShouldThrow_WhenRelationNotFound()
    {
        var command = new SendPeaceOfferCommand(CampaignId.New(), FactionType.England, FactionType.France);

        var action = () => CreateHandler().Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>();
        await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailureAndNotSaveChanges_WhenDomainReturnsFailure()
    {
        var relation = TestData.CreateDiplomacyRelation();
        var command = new SendPeaceOfferCommand(relation.CampaignId, relation.FactionA, relation.FactionB);

        _unitOfWork.DiplomacyRelationRepository.GetAsync(default, default, default, default).ReturnsForAnyArgs(relation);

        var result = await CreateHandler().Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(GameErrors.CantPeaceFactionsNotWar);
        await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }
}