using System.Net;
using System.Net.Http.Json;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

using RTSCore.Domain.Entities;
using RTSCore.Domain.ValueObjects;
using RTSCore.Domain.ValueObjects.Presets;
using RTSCore.Infrastructure.Persistence;
using RTSCore.Domain.Services;
using RTSCore.Tests.Base;
using Microsoft.AspNetCore.Mvc.Testing;
using RTSCore.Application.Campaign.Lifecycle;
using RTSCore.Application.Campaign.Diplomacy.WarDeclaration;
using RTSCore.Application.Campaign.Diplomacy.PeaceNegotiation;
using RTSCore.Application.Campaign.Diplomacy.TradeProposal;
using RTSCore.Application.Campaign.Diplomacy.OfferResponses;

namespace RTSCore.Tests.WebApi.Controllers;

public class WebIntegrationTests(WebApplicationFactory<Program> factory) : WebTestBase(factory)
{
    #region DiplomacyController

    [Fact]
    public async Task DeclareWar_WithValidCommand_ShouldReturnNoContent()
    {
        var initiator = FactionType.England;
        var target = FactionType.France;

        using (var scope = _factory.Services.CreateScope())
        {
            var england = new Faction(initiator, 0, PlayerType.Human);
            var france = new Faction(target, 0, PlayerType.Ai);
            var relation = new DiplomacyRelation(initiator, target, 0);
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            context.Factions.Add(england);
            context.Factions.Add(france);
            context.DiplomacyRelations.Add(relation);

            await context.SaveChangesAsync();
        }

        var command = new DeclareWarCommand(initiator, target);
        var response = await _client.PostAsJsonAsync("api/diplomacy/offers/war", command);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task SendPeaceOffer_WithValidCommand_ShouldReturnOkWithGuid()
    {
        var initiator = FactionType.England;
        var target = FactionType.France;

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var england = new Faction(initiator, 0, PlayerType.Human);
            var france = new Faction(target, 0, PlayerType.Ai);
            var relation = new DiplomacyRelation(initiator, target, 0);
            relation.DeclareWar();

            context.Factions.Add(england);
            context.Factions.Add(france);
            context.DiplomacyRelations.Add(relation);

            await context.SaveChangesAsync();
        }

        var command = new SendPeaceOfferCommand(initiator, target);
        var response = await _client.PostAsJsonAsync("api/diplomacy/offers/peace", command);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var offerId = await response.Content.ReadFromJsonAsync<Guid>();
        Assert.NotEqual(Guid.Empty, offerId);
    }

    [Fact]
    public async Task SendTradeOffer_WithValidCommand_ShouldReturnOkWithGuid()
    {

        var initiator = FactionType.England;
        var target = FactionType.France;

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var england = new Faction(initiator, 0, PlayerType.Human);
            var france = new Faction(target, 0, PlayerType.Ai);
            var relation = new DiplomacyRelation(initiator, target, 0);

            context.Factions.Add(england);
            context.Factions.Add(france);
            context.DiplomacyRelations.Add(relation);

            await context.SaveChangesAsync();
        }

        var command = new SendTradeOfferCommand(initiator, target);
        var response = await _client.PostAsJsonAsync("api/diplomacy/offers/trade", command);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var offerId = await response.Content.ReadFromJsonAsync<Guid>();
        Assert.NotEqual(Guid.Empty, offerId);
    }

    [Fact]
    public async Task AcceptOffer_WithValidCommand_ShouldReturnNoContent()
    {
        var initiator = FactionType.England;
        var target = FactionType.France;
        Guid offerId;

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var england = new Faction(initiator, 0, PlayerType.Human);
            var france = new Faction(target, 0, PlayerType.Ai);
            var relation = new DiplomacyRelation(initiator, target, GameBalance.Diplomacy.MinStandingForTrade);
            var offer = new DiplomacyOffer(initiator, target, OfferType.TradeAgreement);
            offerId = offer.Id;

            context.Factions.Add(england);
            context.Factions.Add(france);
            context.DiplomacyRelations.Add(relation);
            context.DiplomacyOffers.Add(offer);
            await context.SaveChangesAsync();
        }

        var command = new AcceptOfferCommand(offerId);
        var response = await _client.PostAsJsonAsync($"api/diplomacy/offers/{offerId}/accept", command);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task RejectOffer_WithValidCommand_ShouldReturnNoContent()
    {
        var initiator = FactionType.England;
        var target = FactionType.France;
        Guid offerId;

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var england = new Faction(initiator, 0, PlayerType.Ai);
            var france = new Faction(target, 0, PlayerType.Human);
            var relation = new DiplomacyRelation(initiator, target, GameBalance.Diplomacy.InitialStanding);
            var offer = new DiplomacyOffer(initiator, target, OfferType.TradeAgreement);
            offerId = offer.Id;

            context.Factions.Add(england);
            context.Factions.Add(france);
            context.DiplomacyRelations.Add(relation);
            context.DiplomacyOffers.Add(offer);
            await context.SaveChangesAsync();
        }

        var command = new RejectOfferCommand(offerId);
        var response = await _client.PostAsJsonAsync($"api/diplomacy/offers/{offerId}/reject", command);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    #endregion

    #region CampaingController

    [Fact]
    public async Task StartCampaign_WithValidCommand_ShouldReturnNoContent()
    {
        var command = new StartCampaignCommand([FactionType.England]);

        var response = await _client.PostAsJsonAsync("api/campaign/start", command);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task StartCampaing_WithEmptyFactions_ShouldReturnBadRequest()
    {
        var command = new StartCampaignCommand([]);

        var response = await _client.PostAsJsonAsync("api/campaign/start", command);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task EndTurn_ShouldReturnNoContent()
    {
        var factionType = FactionType.England;

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var faction = new Faction(factionType, gold: 0, PlayerType.Ai);

            context.Factions.Add(faction);
            await context.SaveChangesAsync();
        }

        var response = await _client.PostAsync($"api/faction/{factionType}/turn/end", _emptyContent);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    #endregion

    #region Common

    private async Task<IServiceScope> SeedTestWorldAsync(
        CityId cityId, int? entityCost = 0, Building? buildingToRegister = null)
    {
        var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var faction = new Faction(FactionType.England, entityCost ?? 0, PlayerType.Human);
        var cityPreset = new CityPreset(cityId, "London Test", CityType.Settlement, 1000, []);
        var city = new City(cityPreset, faction.Type, new(0f, 0f));

        if (buildingToRegister != null)
        {
            city.RegisterBuilding(buildingToRegister);
        }

        context.Cities.Add(city);
        context.Factions.Add(faction);
        await context.SaveChangesAsync();

        return scope;
    }

    #endregion
}