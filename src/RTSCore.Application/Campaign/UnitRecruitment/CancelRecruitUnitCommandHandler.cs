using MediatR;

using RTSCore.Domain.Exeptions;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.Services;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Campaign.UnitRecruitment;

public class CancelRecruitUnitCommandHandler(
    IUnitOfWork unitOfWork,
    IReadOnlyCollection<UnitTemplate> unitTemplates,
    UnitRecruitmentService recruitmentService
) : IRequestHandler<CancelRecruitUnitCommand>
{
    public async Task Handle(CancelRecruitUnitCommand request, CancellationToken ct)
    {
        var unit = await unitOfWork.UnitRepository.GetAsync(request.Id, ct)
            ?? throw new NotFoundException(
                $"[{nameof(CancelRecruitUnitCommandHandler)}] Юнита {request.Id} не существует"
            );

        var army = await unitOfWork.ArmyRepository.GetAsync(unit.ArmyId, ct)
            ?? throw new NotFoundException("Армия не найдена");

        var faction = await unitOfWork.FactionRepository.GetFactionAsync(unit.Faction, ct)
            ?? throw new NotFoundException(
                $"[{nameof(CancelRecruitUnitCommandHandler)}] Фракции {unit.Faction} не существует"
            );

        var template = unitTemplates.FirstOrDefault(t => t.Type == unit.Type)
            ?? throw new NotFoundException(
                $"[{nameof(CancelRecruitUnitCommandHandler)}] " +
                $"Шаблон юнита для типа {unit.Type} не найден"
            );

        recruitmentService.CancelRecruitUnit(unit, army, faction, template);

        await unitOfWork.SaveChangesAsync(ct);
    }
}