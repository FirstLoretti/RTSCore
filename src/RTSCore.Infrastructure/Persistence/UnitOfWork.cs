using MediatR;

using RTSCore.Application.Common;
using RTSCore.Application.Repositories;
using RTSCore.Domain.Entities;

namespace RTSCore.Infrastructure.Persistence;

public sealed class UnitOfWork(AppDbContext context, IMediator mediator) : IUnitOfWork
{
    public ICityRepository CityRepository => _cityRepository ??= new CityRepository(context);
    public IFactionRepository FactionRepository => _factionRepository ??= new FactionRepository(context);
    public IDiplomacyRelationRepository DiplomacyRelationRepository => _relationRepository ??= new DiplomacyRelationRepository(context);
    public IDiplomacyOfferRepository DiplomacyOfferRepository => _offerRepository ??= new DiplomacyOfferRepository(context);
    public IUserRepository UserRepository => _userRepository ??= new UserRepository(context);
    public IRefreshTokenRepository RefreshTokenRepository => _tokenRepository ??= new RefreshTokenRepository(context);
    public IArmyRepository ArmyRepository => _armyRepository ??= new SqlArmyRepository(context);
    public IAiAgentRepository AiAgentRepository => _aiFactionAgent ??= new AiAgentRepository(context);
    public ICampaignRepository CampaignRepository => _campaignRepository ??= new CampaignRepository(context);

    private CityRepository? _cityRepository;
    private FactionRepository? _factionRepository;
    private DiplomacyRelationRepository? _relationRepository;
    private DiplomacyOfferRepository? _offerRepository;
    private UserRepository? _userRepository;
    private RefreshTokenRepository? _tokenRepository;
    private SqlArmyRepository? _armyRepository;
    private AiAgentRepository? _aiFactionAgent;
    private CampaignRepository? _campaignRepository;

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