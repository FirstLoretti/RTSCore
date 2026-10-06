using MediatR;

using RTSCore.Application.Common;
using RTSCore.Application.Common.Validation;
using RTSCore.Domain.Common;
using RTSCore.Domain.Entities.Common;
using RTSCore.Domain.ValueObjects.Events;

namespace RTSCore.Application.Campaign.CityConstruction.CancelConstruction;

public class RefundGoldOnConstructionCanceled(
    IUnitOfWork unitOfWork
) : INotificationHandler<DomainEventNotification<BuildingConstructionCanceledEvent>>
{
    public async Task Handle(
        DomainEventNotification<BuildingConstructionCanceledEvent> notification,
        CancellationToken ct
    )
    {
        var (cost, factionType) = notification.DomainEvent;

        var faction = await unitOfWork.FactionRepository.GetFactionAsync(factionType, ct);
        Guard.Against.NotFound(faction, factionType);

        faction.RefundGold(cost);
    }
}