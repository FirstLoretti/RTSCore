using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using RTSCore.Domain.Entities;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Infrastructure.Persistence.Configurations;

public class UnitConfigurations : IEntityTypeConfiguration<Unit>
{
    public void Configure(EntityTypeBuilder<Unit> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id).HasConversion(
            id => id.Value,
            dbValue => new UnitId(dbValue)
        );
        builder.Property(u => u.Type).HasConversion<string>();
        builder.Property(u => u.Faction).HasConversion<string>();
    }
}