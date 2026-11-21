using Ardalis.GuardClauses;

using FluentAssertions;

using NSubstitute;

using RTSCore.Application.Campaign.UnitRecruitment;
using RTSCore.Application.Repositories;

namespace Tests.Application.Campaign.UnitRecruitment;

public sealed class GetRecruitOptionsTests
{
    [Fact]
    public async Task Handle_ShouldThrow_WhenCityNotFound()
    {
        var repository = Substitute.For<ICityRepository>();
        var handler = new GetRecruitOptionsQueryHandler(repository, TestData.UnitCatalog);

        var action = () => handler.Handle(new GetRecruitOptionsQuery(default), default);

        await action.Should().ThrowAsync<NotFoundException>();
    }
}