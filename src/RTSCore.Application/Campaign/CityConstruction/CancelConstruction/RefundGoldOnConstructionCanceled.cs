using MediatR;

using RTSCore.Application.Common;
using RTSCore.Domain.Common.Exceptions;
using RTSCore.Domain.Common.Repositories;
using RTSCore.Domain.Events;

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

        var faction = await unitOfWork.FactionRepository.GetAsync(factionType, ct);
        Guard.Against.NotFound(faction, factionType);

        faction.RefundGold(cost);
    }
}