using MediatR;

using RTSCore.Application.Common;
using RTSCore.Domain.Entities;
using RTSCore.Domain.Interfaces;

namespace RTSCore.Application.Buildings.Commands;

public class TrainUnitCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<TrainUnitCommand>
{
    public async Task Handle(TrainUnitCommand request, CancellationToken cancellationToken)
    {
        var building = await unitOfWork.BuildingRepository.GetBuildingAsync(request.BuildingId, cancellationToken)
            ?? throw new NotFoundException($"Здание {request.BuildingId} не найдено в базе данных");

        if (building is not Barrack barrack)
            throw new GameRuleException($"Здание {building.Id} не является казармой и не способно нанимать юнитов");

        if (!barrack.HasFreeRecruitmentSlots)
            throw new GameRuleException($"Здание {barrack.Id} не имеет свободных слотов под найм");

        barrack.AddUnitToQueue();

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}