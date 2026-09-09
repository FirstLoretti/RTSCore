using Microsoft.EntityFrameworkCore;

using RTSCore.Domain.Entities;

namespace RTSCore.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Unit> Units { get; set; }
    public DbSet<Building> Buildings { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<Faction> Factions { get; set; }
    public DbSet<DiplomacyRelation> DiplomacyRelations { get; set; }
    public DbSet<DiplomacyOffer> DiplomacyOffers { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<string>()
            .HaveMaxLength(256);
    }
}