using MediatR;

using RTSCore.Application.Common;
using RTSCore.Domain.Entities;
using RTSCore.Domain.Interfaces;

namespace RTSCore.Application.Buildings.Commands;

public class CancelUnitCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CancelTrainUnitCommand>
{
    public async Task Handle(CancelTrainUnitCommand request, CancellationToken cancellationToken)
    {
        var building = await unitOfWork.BuildingRepository.GetBuildingAsync(request.BuildingId, cancellationToken)
            ?? throw new NotFoundException($"Здание {request.BuildingId} не найдено в базе данных");

        if (building is not Barrack barrack)
            throw new GameRuleException($"Здание {building.Id} не является казармой и не способно нанимать юнитов");

        if (!barrack.CanCancelRecruitment)
            throw new GameRuleException($"У здания {barrack.Id} уже пустая очередь найма");

        barrack.RemoveUnitForQueue();

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}