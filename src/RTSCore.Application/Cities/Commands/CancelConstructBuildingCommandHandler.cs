using MediatR;

using RTSCore.Domain.Exeptions;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.Services;

namespace RTSCore.Application.Cities.Commands;

public class CancelConstructBuildingCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CancelConstructBuildingCommand>
{
    public async Task Handle(CancelConstructBuildingCommand request, CancellationToken cancellationToken)
    {
        var building = await unitOfWork.BuildingRepository.GetBuildingAsync(request.BuildingId, cancellationToken)
            ?? throw new NotFoundException(
                $"[{nameof(CancelConstructBuildingCommandHandler)}] " +
                $"Здания {request.BuildingId} нет на карте кампании"
            );

        if (building.IsConstructed)
            throw new GameRuleException(
                $"[{nameof(CancelConstructBuildingCommandHandler)}] " +
                $"Нельзя отменить строительство, здание {building.Id} уже построено"
            );

        var faction = await unitOfWork.FactionRepository.GetFactionAsync(building.OwnerFaction, cancellationToken)
            ?? throw new NotFoundException(
                $"[{nameof(CancelConstructBuildingCommandHandler)}] " +
                $"Фракции {building.OwnerFaction} нет на карте кампании"
            );

        var template = GameBalance.Buildings.GetTemplate(building.Type);
        faction.RefundGold(template.Cost / 2);
        unitOfWork.BuildingRepository.Remove(building);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}