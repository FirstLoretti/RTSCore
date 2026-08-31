using MediatR;

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
            var growthRate = GameBalance.Population.CalculateGrowthRate(city);
            city.GrowPopulation(growthRate);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}