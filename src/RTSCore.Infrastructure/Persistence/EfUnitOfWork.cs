using MediatR;

using RTSCore.Application.Common;
using RTSCore.Application.Repositories;
using RTSCore.Domain.Entities;

namespace RTSCore.Infrastructure.Persistence;

public sealed class EfUnitOfWork(AppDbContext context, IMediator mediator) : IUnitOfWork
{
    public ICityRepository CityRepository => _cityRepository ??= new SqlCityRepository(context);
    public IFactionRepository FactionRepository => _factionRepository ??= new SqlFactionRepository(context);
    public IDiplomacyRelationRepository DiplomacyRelationRepository => _relationRepository ??= new SqlDiplomacyRelationRepository(context);
    public IDiplomacyOfferRepository DiplomacyOfferRepository => _offerRepository ??= new SqlDiplomacyOfferRepository(context);
    public IUserRepository UserRepository => _userRepository ??= new SqlUserRepository(context);
    public IRefreshTokenRepository RefreshTokenRepository => _tokenRepository ??= new SqlRefreshTokenRepository(context);
    public IArmyRepository ArmyRepository => _armyRepository ??= new SqlArmyRepository(context);

    private SqlCityRepository? _cityRepository;
    private SqlFactionRepository? _factionRepository;
    private SqlDiplomacyRelationRepository? _relationRepository;
    private SqlDiplomacyOfferRepository? _offerRepository;
    private SqlUserRepository? _userRepository;
    private SqlRefreshTokenRepository? _tokenRepository;
    private SqlArmyRepository? _armyRepository;

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