using MediatR;

using RTSCore.Domain.Common;

namespace RTSCore.Application.Common;

public class DomainEventNotification<T>(T domainEvent) : INotification
    where T : IDomainEvent
{
    public T DomainEvent { get; } = domainEvent;
}