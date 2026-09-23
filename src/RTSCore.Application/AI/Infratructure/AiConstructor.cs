using MediatR;

using RTSCore.Application.AI.Brains;
using RTSCore.Application.Campaign.CityConstruction;
using RTSCore.Domain.Exeptions;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.AI.Infratructure;

public class AiConstructor(
    IUnitOfWork unitOfWork,
    IMediator mediator,
    BuildingUtilityCalculator calculator
)
{
    public async Task HandleConstructLogicAsync(FactionType factionType, CancellationToken cancellationToken)
    {
        var cities = await unitOfWork.CityRepository.GetCitiesAsync(factionType, cancellationToken);
        var faction = await unitOfWork.FactionRepository.GetFactionAsync(factionType, cancellationToken)
            ?? throw new NotFoundException($"Фракция {factionType} не найдена в базе данных");

        var options = calculator.GetOrderedOptions(cities);

        var simulateGold = faction.Gold;
        foreach (var option in options)
        {
            if (simulateGold >= option.Cost)
            {
                var command = new ConstructBuildingCommand(option.CityId, option.BuildingType);
                await mediator.Send(command, cancellationToken);
                simulateGold -= option.Cost;
            }
            else break;
        }
    }
}