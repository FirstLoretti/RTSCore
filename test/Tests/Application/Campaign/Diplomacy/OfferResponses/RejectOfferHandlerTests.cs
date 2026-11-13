using Ardalis.GuardClauses;

using FluentAssertions;

using NSubstitute;

using RTSCore.Application.Campaign.Diplomacy.OfferResponses;
using RTSCore.Application.Repositories;
using RTSCore.Domain.Entities.Diplomacy;

namespace Tests.Application.Campaign.Diplomacy.OfferResponses;

public sealed class RejectOfferHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private RejectOfferCommandHandler CreateHandler() => new(_unitOfWork);

    [Fact]
    public async Task Handle_ShouldThrowAndNotSaveToDb_WhenOfferNotExist()
    {
        var command = new RejectOfferCommand(DiplomacyOfferId.New());

        var action = () => CreateHandler().Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>();
        await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }
}