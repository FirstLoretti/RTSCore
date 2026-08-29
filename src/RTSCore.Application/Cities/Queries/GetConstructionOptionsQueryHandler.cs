using MediatR;

using RTSCore.Domain.Entities.Rules;
using RTSCore.Domain.Exeptions;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Cities.Queries;

public class GetConstructionOptionsQueryHandler(
    IUnitOfWork unitOfWork,
    IReadOnlyCollection<BuildingTemplate> buildingTemplates
)
    : IRequestHandler<GetConstructionOptionsQuery, IEnumerable<ConstructionOptionDto>>
{
    public async Task<IEnumerable<ConstructionOptionDto>> Handle(GetConstructionOptionsQuery request, CancellationToken cancellationToken)
    {
        var city = await unitOfWork.CityRepository.GetCityWithBuildingsAsync(request.CityId, cancellationToken)
            ?? throw new NotFoundException(
                $"[{nameof(GetConstructionOptionsQueryHandler)}] Поселения {request.CityId} нет на карте кампании"
            );
        var faction = await unitOfWork.FactionRepository.GetFactionAsync(city.OwnerFaction, cancellationToken)
            ?? throw new NotFoundException(
                $"[{nameof(GetConstructionOptionsQueryHandler)}] Фракции {city.OwnerFaction} нет на карте кампании"
            );

        var availableBuildings = new List<ConstructionOptionDto>();

        foreach (var template in buildingTemplates)
        {
            var isAlreadyBuild = city.Buildings.Any(b => b.Type == template.Type && b.IsConstructed);
            if (isAlreadyBuild) continue;

            if (!BuildingRules.CanConstruct(template, city.Type, city.Buildings, out var reason)) continue;

            var hasEnoughGold = faction.Gold >= template.Cost;
            var availability = hasEnoughGold
                ? ConstructionOptionAvailability.Available
                : ConstructionOptionAvailability.Locked;

            var lockReason = hasEnoughGold ? null : "Недостаточно средств";

            availableBuildings.Add(new ConstructionOptionDto(
                template.Type,
                template.DisplayName,
                template.Cost,
                availability,
                lockReason
            ));
        }

        return availableBuildings;
    }
}