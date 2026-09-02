using MediatR;

using RTSCore.Domain.Exeptions;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Units.Commands;

public class DisbandUnitCommandHandler(IUnitRepository unitRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<DisbandUnitCommand>
{
    public async Task Handle(DisbandUnitCommand request, CancellationToken cancellationToken)
    {
        var unit = await unitRepository.GetUnitAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"[{nameof(DisbandUnitCommand)}]Юнита {request.Id} нет в базе данных");

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

        unitRepository.Delete(unit);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}