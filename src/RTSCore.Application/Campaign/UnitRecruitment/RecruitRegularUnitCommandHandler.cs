using MediatR;

using RTSCore.Domain.Exeptions;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.Services;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Campaign.UnitRecruitment;

public class RecruitRegularUnitCommandHandler(
    IUnitOfWork unitOfWork,
    IReadOnlyCollection<UnitTemplate> units,
    UnitRecruitmentService recruitmentService
) : IRequestHandler<RecruitRegularUnitCommand>
{
    public async Task Handle(RecruitRegularUnitCommand request, CancellationToken ct)
    {
        var unit = units.FirstOrDefault(u => u.Type == request.Type)
            ?? throw new NotFoundException(
                $"[{nameof(RecruitRegularUnitCommandHandler)}] " +
                $"Нет шаблона для юнита типа {request.Type}"
            );

        var army = await unitOfWork.ArmyRepository.GetAsync(request.ArmyId, ct)
            ?? throw new NotFoundException(
                $"[{nameof(RecruitRegularUnitCommandHandler)}] " +
                $"Армии {request.ArmyId} нет на карте кампании"
            );

        var city = await unitOfWork.CityRepository.GetCityByCoordAsync(army.Coordinates, ct)
            ?? throw new NotFoundException("Армия не в городе. Найм невозможен");

        var faction = await unitOfWork.FactionRepository.GetFactionAsync(army.Faction, ct)
            ?? throw new NotFoundException(
                $"[{nameof(RecruitRegularUnitCommandHandler)}] " +
                $"Фракции {army.Faction} нет в текущей игре"
            );

        recruitmentService.RecruitUnit(unit, army, faction, city);

        await unitOfWork.SaveChangesAsync(ct);
    }
}