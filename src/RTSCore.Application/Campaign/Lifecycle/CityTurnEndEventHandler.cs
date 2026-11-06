using Ardalis.GuardClauses;

using MediatR;

using RTSCore.Application.Common;
using RTSCore.Application.Repositories;
using RTSCore.Domain.Entities.Cities;

namespace RTSCore.Application.Campaign.Lifecycle;

public sealed class CityTurnEndEventHandler(IUnitOfWork unitOfWork) : INotificationHandler<DomainEventNotification<CityTurnEndEvent>>
{
    public async Task Handle(DomainEventNotification<CityTurnEndEvent> notification, CancellationToken ct)
    {
        var faction = await unitOfWork.FactionRepository.GetAsync(
            notification.DomainEvent.CampaignId,
            notification.DomainEvent.Faction,
            ct
        );
        Guard.Against.NotFound(notification.DomainEvent.Faction, faction);

        faction.EarnGold(notification.DomainEvent.BuildingsIncome + notification.DomainEvent.TaxesIncome);
    }
}