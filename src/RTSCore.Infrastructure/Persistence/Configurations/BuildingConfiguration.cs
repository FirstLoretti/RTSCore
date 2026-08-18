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

        builder.Property(b => b.Type).HasConversion<string>();
        builder.Property(b => b.Faction).HasConversion<string>();

        builder.HasDiscriminator<string>("Discriminator")
            .HasValue<Building>("Building")
            .HasValue<Barrack>("Barrack");
    }
}