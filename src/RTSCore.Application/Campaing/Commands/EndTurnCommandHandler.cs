using MediatR;

using RTSCore.Domain.Exeptions;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.Services;

namespace RTSCore.Application.Campaing.Commands;

public class EndTurnCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<EndTurnCommand>
{
    public async Task Handle(EndTurnCommand request, CancellationToken cancellationToken)
    {
        var activeConstructions = await unitOfWork.BuildingRepository.GetUnderConstructionAsync(cancellationToken);

        foreach (var construction in activeConstructions)
        {
            construction.AdvanceConstruction();
        }

        var cities = await unitOfWork.CityRepository.GetCitiesWithBuildingsAsync(cancellationToken);

        foreach (var city in cities)
        {
            var faction = await unitOfWork.FactionRepository.GetFactionAsync(city.OwnerFaction, cancellationToken)
                ?? throw new NotFoundException(
                    $"[{nameof(EndTurnCommandHandler)}] " +
                    $"Фракции {city.OwnerFaction} нет на карте кампании"
                );

            var taxIncome = city.CalculateTaxIncome(GameBalance.Economy.TaxRatePerCitizen);
            var buildingsIncome = city.CalculateBuildingsIncome();

            faction.EarnGold(taxIncome + buildingsIncome);

            var growthRate = GameBalance.Population.CalculateGrowthRate(city);
            city.GrowPopulation(growthRate);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}