using MediatR;

using RTSCore.Application.Common;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Units.Commands;

public class DeleteUnitCommandHandler(IUnitRepository unitRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteUnitCommand>
{
    public async Task Handle(DeleteUnitCommand request, CancellationToken cancellationToken)
    {
        var unit = await unitRepository.GetUnitAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Юнита {request.Id} нет в базе данных");

        if (unit.Type == UnitType.Invulnerable)
        {
            throw new GameRuleException(
                $"Юнита {unit.Id} с типом {unit.Type} нельзя удалить из базы данных"
            );
        }

        unitRepository.Delete(unit);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}