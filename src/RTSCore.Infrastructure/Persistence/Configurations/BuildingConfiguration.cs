using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using RTSCore.Domain.Entities;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Infrastructure.Persistence.Configurations;

public class BuildingConfiguration : IEntityTypeConfiguration<Building>
{
    public void Configure(EntityTypeBuilder<Building> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id).HasConversion(
            id => id.Value,
            dbValue => new BuildingId(dbValue)
        );

        builder.Property(b => b.CityId).HasConversion(
            id => id.Value,
            dbValue => new CityId(dbValue)
        );

        builder.Property(b => b.Type).HasConversion<string>();
        builder.Property(b => b.OwnerFaction).HasConversion<string>();
        builder.Property(b => b.IsConstructed).HasConversion<bool>();
        builder.Property(b => b.TurnsToConstruct).HasConversion<int>();

        builder.HasOne<City>()
            .WithMany(c => c.Buildings)
            .HasForeignKey(b => b.CityId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}