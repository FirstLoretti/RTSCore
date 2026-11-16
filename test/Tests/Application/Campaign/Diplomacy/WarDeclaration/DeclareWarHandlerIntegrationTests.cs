using FluentAssertions;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using RTSCore.Application.Campaign.Diplomacy.WarDeclaration;
using RTSCore.Infrastructure.Persistence;

namespace Tests.Application.Campaign.Diplomacy.WarDeclaration;

[Collection("TestCollection")]
public sealed class DeclareWarHandlerIntegrationTests(TestFixture testFixture) : IAsyncLifetime
{
    [Fact]
    public async Task Handle_ShouldDeclareWarAndSaveToDb()
    {
        var relation = TestData.CreateDiplomacyRelation();

        using (var scope = testFixture.ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            context.DiplomacyRelations.Add(relation);

            await context.SaveChangesAsync();
        }

        using (var scope = testFixture.ServiceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var result = await mediator.Send(new DeclareWarCommand(relation.CampaignId, relation.FactionA, relation.FactionB));

            result.IsSuccess.Should().BeTrue();
        }

        using (var scope = testFixture.ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var dbRelation = await context.DiplomacyRelations
                .AsNoTracking()
                .FirstAsync(r => r.Id == relation.Id);

            dbRelation.InWar.Should().BeTrue();
            dbRelation.Standing.Should().Be(relation.Standing + TestData.DiplomacyConfig.DeclareWarPenalty);
        }
    }

    public Task DisposeAsync() => Task.CompletedTask;

    public Task InitializeAsync() => testFixture.ResetDatabaseAsync();
}