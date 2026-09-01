using MediatR;

using RTSCore.Domain.Exeptions;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.Services;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Cities.Commands;

public class RecruitUnitCommandHandler(
    IUnitRepository repository,
    IUnitOfWork unitOfWork,
    IReadOnlyCollection<UnitTemplate> unitTemplates
) : IRequestHandler<RecruitUnitCommand>
{
    public async Task Handle(RecruitUnitCommand request, CancellationToken cancellationToken)
    {
        var template = unitTemplates.FirstOrDefault(u => u.Type == request.Type)
            ?? throw new NotFoundException(
                $"[{nameof(RecruitUnitCommandHandler)}] " +
                $"Шаблон для юнита типа {request.Type} не содержится в {nameof(GameBalance.Units)}"
            );

        var city = await unitOfWork.CityRepository.GetCityWithBuildingsAsync(request.CityId, cancellationToken)
            ?? throw new NotFoundException(
                $"[{nameof(RecruitUnitCommandHandler)}] " +
                $"Поселения {request.CityId} нет на карте кампании"
            );

        if (city.OwnerFaction != request.OwnerFaction)
        {
            throw new GameRuleException(
                $"[{nameof(RecruitUnitCommandHandler)}] " +
                $"Фракция {request.OwnerFaction} не может нанимать в поселении фракции {city.OwnerFaction}"
            );
        }

        var faction = await unitOfWork.FactionRepository.GetFactionAsync(city.OwnerFaction, cancellationToken)
            ?? throw new NotFoundException(
                $"[{nameof(RecruitUnitCommandHandler)}] " +
                $"Фракции {city.OwnerFaction} нет в текущей игре"
            );

        if (faction.Gold < template.Cost)
        {
            throw new GameRuleException(
                $"[{nameof(RecruitUnitCommandHandler)}] Недостаточно денег для найма {template.DisplayName}"
            );
        }

        if (template.RequiredBuilding is BuildingType requiredBuilding)
        {
            var hasRequiredBuilding = city.Buildings.Any(b => b.Type == requiredBuilding && b.IsConstructed);
            if (!hasRequiredBuilding)
            {
                throw new GameRuleException(
                    $"[{nameof(RecruitUnitCommandHandler)}] Нельзя нанять {request.Type}. " +
                    $"В городе {request.CityId} отсутствует здание типа {nameof(requiredBuilding)}"
                );
            }
        }

        faction.SpendGold(template.Cost);

        var unitId = new UnitId($"unit_{request.Type}_{Guid.NewGuid().ToString("N")[..5]}");
        var unit = new Domain.Entities.Unit(unitId, request.OwnerFaction, template, request.CityId);

        repository.Add(unit);

        await unitOfWork.SaveChangesAsync(cancellationToken);

    }
}