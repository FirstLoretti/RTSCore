using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using RTSCore.Domain.Entities.Armies;
using RTSCore.Domain.Entities.Campaigns;

namespace RTSCore.Infrastructure.Persistence.Configurations;

public sealed class ArmyConfiguration : IEntityTypeConfiguration<Army>
{
    public void Configure(EntityTypeBuilder<Army> builder)
    {
        builder.Property(a => a.CampaignId).HasConversion(
            id => id.Value,
            dbValue => new CampaignId(dbValue)
        );

        builder.Property(a => a.Id).HasConversion(
            id => id.Value,
            dbValue => new ArmyId(dbValue)
        );

        builder.Property(a => a.GeneralId).HasConversion(
            id => id.Value,
            dbValue => new UnitId(dbValue)
        );

        builder.Property(a => a.Faction).HasConversion<string>();

        builder.ComplexProperty(a => a.Coordinates);

        builder.HasMany(a => a.Units)
            .WithOne()
            .HasForeignKey(u => u.ArmyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}