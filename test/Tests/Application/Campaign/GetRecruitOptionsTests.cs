using Ardalis.GuardClauses;

using FluentAssertions;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

using RTSCore.Application.Campaign.UnitRecruitment;
using RTSCore.Domain.Entities.Armies;
using RTSCore.Domain.Entities.Cities;
using RTSCore.Infrastructure.Persistence;

namespace Tests.Application.Campaign;

[Collection("TestCollection")]
public sealed class GetRecruitOptionsTests(TestFixture testFixture) : IAsyncLifetime
{
    [Fact]
    public async Task Handle_ShouldThrow_WhenCityNotFound()
    {
        using var scope = testFixture.ServiceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var action = () => mediator.Send(new GetRecruitOptionsQuery(CityId.New()));

        await action.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_ShouldGetRecruitOptions()
    {
        var city = TestData.CreateCity(template: TestData.CreateCityTemplate());
        city.ConstructInstantly(TestData.CreateBuildingTemplate());

        using (var scope = testFixture.ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Cities.Add(city);
            await context.SaveChangesAsync();
        }

        using (var scope = testFixture.ServiceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var options = await mediator.Send(new GetRecruitOptionsQuery(city.Id));

            options.Should().ContainSingle().Which.Type.Should().Be(UnitType.Peasant);
        }
    }

    public Task InitializeAsync() => testFixture.ResetDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;
}