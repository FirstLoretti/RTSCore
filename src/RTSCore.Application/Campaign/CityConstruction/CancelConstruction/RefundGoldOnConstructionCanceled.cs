using Ardalis.GuardClauses;

using MediatR;

using RTSCore.Application.Common;
using RTSCore.Application.Repositories;
using RTSCore.Domain.Entities.Cities;

namespace RTSCore.Application.Campaign.CityConstruction.CancelConstruction;

public sealed class RefundGoldOnConstructionCanceled(
    IUnitOfWork unitOfWork
) : INotificationHandler<DomainEventNotification<ConstructionCanceledEvent>>
{
    public async Task Handle(
        DomainEventNotification<ConstructionCanceledEvent> notification,
        CancellationToken ct
    )
    {
        var (factionType, cost) = notification.DomainEvent;

        var faction = await unitOfWork.FactionRepository.GetAsync(factionType, ct);
        Guard.Against.NotFound(factionType, faction);

        faction.RefundGold(cost);
    }
}