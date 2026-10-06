using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using RTSCore.Domain.Entities.Campaign;
using RTSCore.Domain.ValueObjects.Identifiers;

namespace RTSCore.Infrastructure.Persistence.Configurations;

public class ArmyConfiguration : IEntityTypeConfiguration<Army>
{
    public void Configure(EntityTypeBuilder<Army> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.GeneralId).HasConversion(
            id => id.Value,
            dbValue => new UnitId(dbValue)
        );

        builder.Property(a => a.Faction).HasConversion<string>();

        builder.ComplexProperty(a => a.Coordinates);

        builder.HasMany(a => a.Units)
            .WithOne()
            .HasForeignKey(u => u.ArmyId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}