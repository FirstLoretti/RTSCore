using MediatR;

using RTSCore.Domain.Exeptions;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.Services;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Cities.Commands;

public class CancelRecruitUnitCommandHandler(
    IUnitOfWork unitOfWork,
    IReadOnlyCollection<UnitTemplate> unitTemplates
) : IRequestHandler<CancelRecruitUnitCommand>
{
    public async Task Handle(CancelRecruitUnitCommand request, CancellationToken cancellationToken)
    {
        var unitId = request.UnitId;

        var unit = await unitOfWork.UnitRepository.GetUnitAsync(request.UnitId, cancellationToken)
            ?? throw new NotFoundException(
                $"[{nameof(CancelRecruitUnitCommandHandler)}] Юнита {unitId} не существует"
            );

        if (unit.IsRecruited)
        {
            throw new GameRuleException(
               $"[{nameof(CancelRecruitUnitCommandHandler)}] Нельзя отменить найм. Юнит {unitId} уже нанят"
           );
        }

        var faction = await unitOfWork.FactionRepository.GetFactionAsync(unit.OwnerFaction, cancellationToken)
            ?? throw new NotFoundException(
                $"[{nameof(CancelRecruitUnitCommandHandler)}] Фракции {unit.OwnerFaction} не существует"
            );

        var template = unitTemplates.FirstOrDefault(t => t.Type == unit.Type)
            ?? throw new NotFoundException(
                $"[{nameof(CancelRecruitUnitCommandHandler)}] " +
                $"Шаблон юнита для типа {unit.Type} не содержится в {nameof(GameBalance.Units)}"
            );

        faction.EarnGold(template.Cost);
        unitOfWork.UnitRepository.Delete(unit);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}