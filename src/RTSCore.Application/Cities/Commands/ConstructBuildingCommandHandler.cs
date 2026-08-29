using MediatR;

using RTSCore.Domain.Entities;
using RTSCore.Domain.Entities.Rules;
using RTSCore.Domain.Exeptions;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.Services;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Cities.Commands;

public class ConstructBuildingCommandHandler(
    IUnitOfWork unitOfWork,
    IReadOnlyCollection<BuildingTemplate> buildingTemplates
) : IRequestHandler<ConstructBuildingCommand>
{
    public async Task Handle(ConstructBuildingCommand request, CancellationToken cancellationToken)
    {
        var city = await unitOfWork.CityRepository.GetCityWithBuildingsAsync(request.CityId, cancellationToken)
            ?? throw new NotFoundException(
                $"[{nameof(ConstructBuildingCommandHandler)}] " +
                $"Поселения {request.CityId} нет на карте кампании"
            );

        var player = await unitOfWork.FactionRepository.GetFactionAsync(city.OwnerFaction, cancellationToken)
            ?? throw new NotFoundException(
                $"[{nameof(ConstructBuildingCommandHandler)}] " +
                $"Поселение {city.Id} принадлежит {city.OwnerFaction}, " +
                $"но эта фракция не зарегистрирована в текущей игре."
            );

        var template = buildingTemplates.FirstOrDefault(b => b.Type == request.BuildingType)
            ?? throw new NotFoundException(
                $"[{nameof(ConstructBuildingCommandHandler)}] " +
                $"Шаблон здания для типа {request.BuildingType} не содержится в {nameof(GameBalance.Buildings)}"
            );

        if (!BuildingRules.CanConstruct(template, city.Type, city.Buildings, out var lockReason))
        {
            throw new GameRuleException(
                $"[{nameof(ConstructBuildingCommandHandler)}] Нарушение цепочки строительства: {lockReason}"
            );
        }

        player.SpendGold(template.Cost);

        var buildingId = new BuildingId($"building_{city.Id}_{request.BuildingType.ToString().ToLower()}");
        var building = new Building(
            buildingId,
            request.BuildingType,
            city.OwnerFaction,
            city.Id
        );

        unitOfWork.BuildingRepository.Add(building);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}