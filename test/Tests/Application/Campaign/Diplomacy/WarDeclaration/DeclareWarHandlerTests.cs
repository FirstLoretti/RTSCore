using Ardalis.GuardClauses;

using FluentAssertions;

using NSubstitute;

using RTSCore.Application.Campaign.Diplomacy.WarDeclaration;
using RTSCore.Application.Repositories;
using RTSCore.Domain;
using RTSCore.Domain.Entities.Campaigns;
using RTSCore.Domain.Errors;

namespace Tests.Application.Campaign.Diplomacy.WarDeclaration;

public sealed class DeclareWarHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private DeclareWarCommandHandler CreateHandler() => new(_unitOfWork, TestData.DiplomacyConfig);

    [Fact]
    public async Task Handle_ShouldThrow_WhenRelationNotFound()
    {
        var command = new DeclareWarCommand(CampaignId.New(), FactionType.England, FactionType.France);

        var action = () => CreateHandler().Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>();
        await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailureAndNotSaveChanges_WhenDomainReturnsFailure()
    {
        var relation = TestData.CreateDiplomacyRelation();
        relation.DeclareWar(standingPenalty: 0);
        var command = new DeclareWarCommand(relation.CampaignId, relation.FactionA, relation.FactionB);

        _unitOfWork.DiplomacyRelationRepository.GetAsync(default, default, default, default, true).ReturnsForAnyArgs(relation);

        var result = await CreateHandler().Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(GameErrors.CantWarFactionsAlreadyWar);
        await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }
}