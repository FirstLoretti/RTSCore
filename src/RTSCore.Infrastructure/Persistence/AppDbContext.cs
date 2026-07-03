using Microsoft.EntityFrameworkCore;

using RTSCore.Domain.Entities;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Unit> Units { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Unit>(
            builder =>
                {
                    builder.HasKey(u => u.Id);

                    builder.Property(u => u.Id).HasConversion(
                        id => id.Value,
                        dbValue => new UnitId(dbValue)
                    );
                    builder.Property(u => u.Type).HasConversion<string>();
                    builder.Property(u => u.Faction).HasConversion<string>();
                    builder.Property(u => u.Health).HasConversion<string>();
                    builder.Property(u => u.Damage).HasConversion<string>();
                    builder.Property(u => u.Armor).HasConversion<string>();
                    builder.Property(u => u.Level).HasConversion<string>();
                    builder.Property(u => u.Experience).HasConversion<string>();
                }
        );
    }
}