using MediatR;

using RTSCore.Domain.Exeptions;
using RTSCore.Domain.Interfaces;

namespace RTSCore.Application.Campaign.DisbandUnit;

public class DisbandUnitCommandHandler(
    IUnitOfWork unitOfWork
) : IRequestHandler<DisbandUnitCommand>
{
    public async Task Handle(DisbandUnitCommand request, CancellationToken cancellationToken)
    {
        var unit = await unitOfWork.UnitRepository.GetAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"[{nameof(DisbandUnitCommand)}]Юнита {request.Id} нет в базе данных");

        var army = await unitOfWork.ArmyRepository.GetAsync(unit.ArmyId, cancellationToken)
            ?? throw new NotFoundException("Армии нет в базе данных");

        army.DisbandUnit(unit);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}