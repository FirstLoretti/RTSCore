using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using RTSCore.Domain.Entities;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Infrastructure.Persistence.Configurations;

public class ArmyConfiguration : IEntityTypeConfiguration<Army>
{
    public void Configure(EntityTypeBuilder<Army> builder)
    {
        builder.Property(a => a.GeneralId).HasConversion(
            id => id.Value,
            dbValue => new UnitId(dbValue)
        );

        builder.Property(a => a.Faction).HasConversion<string>();

        builder.ComplexProperty(a => a.Coordinates);
    }
}