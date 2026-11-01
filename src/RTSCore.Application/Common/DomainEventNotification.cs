using MediatR;

using RTSCore.Domain.Common;

namespace RTSCore.Application.Common;

public sealed record DomainEventNotification<TDomainEvent>(TDomainEvent DomainEvent) : INotification
    where TDomainEvent : IDomainEvent;