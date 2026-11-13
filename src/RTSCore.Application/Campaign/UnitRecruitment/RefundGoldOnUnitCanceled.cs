using Ardalis.GuardClauses;

using MediatR;

using RTSCore.Application.Common;
using RTSCore.Application.Repositories;
using RTSCore.Domain.Entities.Armies;

namespace RTSCore.Application.Campaign.UnitRecruitment;

public sealed class RefundGoldOnUnitCanceled(
    IUnitOfWork unitOfWork
) : INotificationHandler<DomainEventNotification<CancelRecruitmentEvent>>
{
    public async Task Handle(
        DomainEventNotification<CancelRecruitmentEvent> notification,
        CancellationToken ct
    )
    {
        var (campaignId, factionType, cost) = notification.DomainEvent;

        var faction = await unitOfWork.FactionRepository.GetAsync(campaignId, factionType, ct);
        Guard.Against.NotFound(factionType, faction);

        faction.RefundGold(cost);
    }
}