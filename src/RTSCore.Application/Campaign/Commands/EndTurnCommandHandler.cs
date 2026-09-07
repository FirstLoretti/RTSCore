using MediatR;

using RTSCore.Application.Common;
using RTSCore.Domain.Common;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.Services;

namespace RTSCore.Application.Campaign.Commands;

public class EndTurnCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<EndTurnCommand>
{
    public async Task Handle(EndTurnCommand request, CancellationToken cancellationToken)
    {
        var activeConstructions = await unitOfWork.BuildingRepository.GetUnderConstructionAsync(
            request.Faction, cancellationToken
        );

        for (int i = 0; i < activeConstructions.Count; i++)
        {
            activeConstructions[i].AdvanceConstruction();
        }

        var faction = await unitOfWork.FactionRepository.GetFactionAsync(request.Faction, cancellationToken);
        Guard.Against.NotFound(faction, request.Faction);

        var cities = await unitOfWork.CityRepository.GetWithBuildingsAsync(request.Faction, cancellationToken);

        int cityIncome = 0;
        for (int i = 0; i < cities.Count; i++)
        {
            var taxIncome = cities[i].CalculateTaxIncome(GameBalance.Economy.TaxRatePerCitizen);
            var buildingsIncome = cities[i].CalculateBuildingsIncome();

            cityIncome += taxIncome + buildingsIncome;

            var growthRate = GameBalance.Population.CalculateGrowthRate(cities[i]);
            cities[i].GrowPopulation(growthRate);
        }

        var tradeAgreements = await unitOfWork.DiplomacyRelationRepository.GetActiveTradeAgreementsForFaction(
            request.Faction, cancellationToken
        );

        var tradePartners = tradeAgreements
            .Where(r => r.FactionA == request.Faction || r.FactionB == request.Faction)
            .Select(r => r.FactionA == request.Faction ? r.FactionB : r.FactionA)
            .ToArray();

        int tradeIncome = 0;
        if (tradeAgreements.Count > 0)
        {
            var citiesCountByPartner = await unitOfWork.CityRepository.GetFactionToCityCount(
                tradePartners, cancellationToken
            );
            var partnerCitiesCount = citiesCountByPartner.Values.Sum();

            tradeIncome += partnerCitiesCount * GameBalance.Diplomacy.TradeIncomePerPartnerCity;
        }

        faction.EarnGold(cityIncome + tradeIncome);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}