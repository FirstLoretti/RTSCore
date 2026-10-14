using MediatR;

using RTSCore.Domain.Common.Repositories;
using RTSCore.Domain.Configurations.Armies;
using RTSCore.Domain.Entities.Armies;
using RTSCore.Domain.Exeptions;

namespace RTSCore.Application.Campaign.UnitRecruitment;

public class CancelRecruitUnitCommandHandler(
    IUnitOfWork unitOfWork,
    IReadOnlyCollection<UnitTemplate> templates,
    UnitRecruitmentService recruitmentService
) : IRequestHandler<CancelRecruitUnitCommand>
{
    public async Task Handle(CancelRecruitUnitCommand request, CancellationToken ct)
    {
        var unit = await unitOfWork.UnitRepository.GetAsync(request.Id, ct)
            ?? throw new NotFoundException(
                $"[{nameof(CancelRecruitUnitCommandHandler)}] Юнита {request.Id} не существует"
            );

        var template = templates.FirstOrDefault(t => t.Type == unit.Type)
          ?? throw new NotFoundException(
              $"[{nameof(CancelRecruitUnitCommandHandler)}] " +
              $"Шаблон юнита для типа {unit.Type} не найден"
            );

        var army = await unitOfWork.ArmyRepository.GetAsync(unit.ArmyId, ct)
            ?? throw new NotFoundException("Армия не найдена");

        var faction = await unitOfWork.FactionRepository.GetAsync(unit.Faction, ct)
            ?? throw new NotFoundException(
                $"[{nameof(CancelRecruitUnitCommandHandler)}] Фракции {unit.Faction} не существует"
            );


        recruitmentService.CancelRecruitUnit(unit, army, faction, template);

        await unitOfWork.SaveChangesAsync(ct);
    }
}