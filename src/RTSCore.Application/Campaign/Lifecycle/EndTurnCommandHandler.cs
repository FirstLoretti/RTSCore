using Ardalis.GuardClauses;

using MediatR;

using RTSCore.Application.Repositories;
using RTSCore.Domain;
using RTSCore.Domain.AI;
using RTSCore.Domain.Common;
using RTSCore.Domain.Configurations.Armies;
using RTSCore.Domain.Configurations.Cities;
using RTSCore.Domain.Entities.Armies;
using RTSCore.Domain.Entities.Campaigns;
using RTSCore.Domain.Entities.Cities;

namespace RTSCore.Application.Campaign.Lifecycle;

public sealed class EndTurnCommandHandler(
    IUnitOfWork unitOfWork,
    BuildingCatalog buildingCatalog,
    UnitCatalog unitCatalog,
    CityConfiguration cityConfiguration,
    UnitStatsCalculator statsCalculator,
    AiTurnOrchestrator turnOrchestrator
) : IRequestHandler<EndTurnCommand, Result>
{
    public async Task<Result> Handle(EndTurnCommand request, CancellationToken ct)
    {
        var campaign = await unitOfWork.CampaignRepository.GetAsync(request.CampaignId, ct);
        Guard.Against.NotFound(request.CampaignId, campaign);

        var advanceResult = campaign.AdvanceTurn(request.Faction);
        if (advanceResult.IsFailure) return advanceResult.Error;

        if (!campaign.IsAiTurn)
        {
            var cities = await unitOfWork.CityRepository.GetAsync(request.CampaignId, request.Faction, ct);
            var armies = await unitOfWork.ArmyRepository.GetAsync(request.CampaignId, request.Faction, ct);

            EndTurn(cities, armies);

            await unitOfWork.SaveChangesAsync(ct);
            return Result.Success();
        }

        var worldState = new CampaignWorldState(
            await unitOfWork.FactionRepository.GetAsync(request.CampaignId, ct),
            await unitOfWork.CityRepository.GetAsync(request.CampaignId, ct),
            await unitOfWork.ArmyRepository.GetAsync(request.CampaignId, ct),
            await unitOfWork.DiplomacyRelationRepository.GetAsync(request.CampaignId, ct),
            await unitOfWork.DiplomacyOfferRepository.GetPendingOffersAsync(request.CampaignId, ct),
            statsCalculator,
            unitCatalog
        );

        EndTurnAndRefreshWorldState(worldState, request.Faction);

        var agents = await unitOfWork.AiAgentRepository.GetAsync(request.CampaignId, ct);
        var agentsByFaction = agents.ToDictionary(a => a.FactionType);

        while (campaign.IsAiTurn)
        {
            var currentFaction = campaign.FactionTurn;
            var currentAgent = agentsByFaction[currentFaction];

            turnOrchestrator.SimulateTurn(currentAgent, worldState);
            EndTurnAndRefreshWorldState(worldState, currentFaction);

            campaign.AdvanceTurn(currentFaction);
        }

        unitOfWork.DiplomacyOfferRepository.AddRange(worldState.CreatedOffers);

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }

    private void EndTurn(IReadOnlyList<City> cities, IReadOnlyList<Army> armies)
    {
        foreach (var city in cities) city.TurnEnd(buildingCatalog, cityConfiguration);
        foreach (var army in armies) army.TurnEnd();
    }

    private void EndTurnAndRefreshWorldState(CampaignWorldState worldState, FactionType faction)
    {
        EndTurn(worldState.GetCities(faction), worldState.GetArmies(faction));
        worldState.RefreshFactionContext(faction, statsCalculator, unitCatalog);
    }
}