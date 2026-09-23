using MediatR;

using RTSCore.Domain.Exeptions;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Campaign.DisbandUnit;

public class DisbandUnitCommandHandler(
    IUnitRepository unitRepository,
    IArmyRepository armyRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<DisbandUnitCommand>
{
    public async Task Handle(DisbandUnitCommand request, CancellationToken cancellationToken)
    {
        var unit = await unitRepository.GetUnitAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"[{nameof(DisbandUnitCommand)}]Юнита {request.Id} нет в базе данных");

        var army = await armyRepository.GetAsync(unit.ArmyId, cancellationToken)
            ?? throw new NotFoundException("Армии нет в базе данных");

        if (unit.Type == UnitType.Invulnerable)
        {
            throw new GameRuleException(
                $"[{nameof(DisbandUnitCommand)}] Юнита {unit.Id} с типом {unit.Type} нельзя удалить из базы данных"
            );
        }

        if (!unit.IsRecruited)
        {
            throw new GameRuleException(
                $"[{nameof(DisbandUnitCommand)}] Ненанятого юнита {unit.Id} нельзя распустить"
            );
        }

        army.DisbandUnit(unit);

        unitRepository.Delete(unit);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}