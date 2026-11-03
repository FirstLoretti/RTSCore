using MediatR;

using RTSCore.Application.Common;
using RTSCore.Application.Repositories;
using RTSCore.Domain.Entities;

namespace RTSCore.Infrastructure.Persistence;

public sealed class EfUnitOfWork(AppDbContext context, IMediator mediator) : IUnitOfWork
{
    public ICityRepository CityRepository { get; } = new SqlCityRepository(context);
    public IFactionRepository FactionRepository { get; } = new SqlFactionRepository(context);
    public IDiplomacyRelationRepository DiplomacyRelationRepository { get; } = new SqlDiplomacyRelationRepository(context);
    public IDiplomacyOfferRepository DiplomacyOfferRepository { get; } = new SqlDiplomacyOfferRepository(context);
    public IUserRepository UserRepository { get; } = new SqlUserRepository(context);
    public IRefreshTokenRepository RefreshTokenRepository { get; } = new SqlRefreshTokenRepository(context);
    public IArmyRepository ArmyRepository { get; } = new SqlArmyRepository(context);

    public async Task SaveChangesAsync(CancellationToken ct)
    {
        var domainEntities = context.ChangeTracker
            .Entries<AggregateRoot>()
            .Where(e => e.Entity.DomainEvents.Count != 0)
            .Select(e => e.Entity)
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        domainEntities.ForEach(e => e.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
        {
            var notificationType = typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType());
            var notification = Activator.CreateInstance(notificationType, domainEvent);
            if (notification != null)
            {
                await mediator.Publish(notification, ct);
            }
        }

        await context.SaveChangesAsync(ct);
    }
}