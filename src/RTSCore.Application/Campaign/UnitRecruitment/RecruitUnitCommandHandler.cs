using MediatR;

using RTSCore.Domain.Exeptions;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.Services;
using RTSCore.Domain.ValueObjects;

using Unit = RTSCore.Domain.Entities.Unit;

namespace RTSCore.Application.Campaign.UnitRecruitment;

public class RecruitUnitCommandHandler(
    IUnitOfWork unitOfWork,
    IReadOnlyCollection<UnitTemplate> unitTemplates
) : IRequestHandler<RecruitUnitCommand>
{
    public async Task Handle(RecruitUnitCommand request, CancellationToken cancellationToken)
    {
        var army = await unitOfWork.ArmyRepository.GetAsync(request.ArmyId, cancellationToken)
            ?? throw new NotFoundException(
                $"[{nameof(RecruitUnitCommandHandler)}] " +
                $"Армии {request.ArmyId} нет на карте кампании"
            );

        var city = await unitOfWork.CityRepository.GetCityByCoordAsync(army.Coordinates, cancellationToken)
            ?? throw new NotFoundException("Армия не в городе. Найм невозможен");

        var template = unitTemplates.FirstOrDefault(u => u.Type == request.Type)
            ?? throw new NotFoundException(
                $"[{nameof(RecruitUnitCommandHandler)}] " +
                $"Шаблон для юнита типа {request.Type} не содержится в {nameof(GameBalance.Units)}"
            );

        if (army.Faction != request.Faction) throw new GameRuleException("Фракция отряда и армии не совпадает.");
        if (!army.HasFreeSlots) throw new GameRuleException("У армии нет свободных слотов для найма");

        var faction = await unitOfWork.FactionRepository.GetFactionAsync(army.Faction, cancellationToken)
            ?? throw new NotFoundException(
                $"[{nameof(RecruitUnitCommandHandler)}] " +
                $"Фракции {army.Faction} нет в текущей игре"
            );

        if (faction.Gold < template.Cost) throw new GameRuleException("Недостаточно денег для найма отряда.");

        var allowedUnit = city.GetAvailableRecruitOptions([request.Type]);
        if (allowedUnit.Length == 0)
            throw new GameRuleException($"Юнит {request.Type} недоступен для найма в городе {city}");

        faction.SpendGold(template.Cost);

        army.RecruitUnit(template);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}