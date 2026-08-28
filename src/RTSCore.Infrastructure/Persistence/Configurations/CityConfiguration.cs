using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using RTSCore.Domain.Entities;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Infrastructure.Persistence.Configurations;

public class CityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id).HasConversion(
            id => id.Value,
            dbValue => new CityId(dbValue)
        );

        builder.Property(c => c.Type).HasConversion<string>();
        builder.Property(c => c.OwnerFaction).HasConversion<string>();
        builder.Property(c => c.Population).HasConversion<int>();
    }
}