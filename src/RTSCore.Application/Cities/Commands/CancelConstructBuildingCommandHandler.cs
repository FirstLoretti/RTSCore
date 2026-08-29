using MediatR;

using RTSCore.Domain.Exeptions;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.Services;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Cities.Commands;

public class CancelConstructBuildingCommandHandler(
    IUnitOfWork unitOfWork,
    IReadOnlyCollection<BuildingTemplate> buildingTemplates
) : IRequestHandler<CancelConstructBuildingCommand>
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

        var template = buildingTemplates.FirstOrDefault(b => b.Type == building.Type)
            ?? throw new NotFoundException(
                $"[{nameof(ConstructBuildingCommandHandler)}] " +
                $"Шаблон здания для типа {building.Type} не содержится в {nameof(GameBalance.Buildings)}"
            );

        faction.RefundGold(template.Cost);
        unitOfWork.BuildingRepository.Remove(building);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}