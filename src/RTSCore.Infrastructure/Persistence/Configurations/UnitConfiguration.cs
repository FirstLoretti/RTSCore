using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using RTSCore.Domain.Entities.Armies;
using RTSCore.Domain.Entities.Campaigns;

namespace RTSCore.Infrastructure.Persistence.Configurations;

public sealed class UnitConfigurations : IEntityTypeConfiguration<Unit>
{
    public void Configure(EntityTypeBuilder<Unit> builder)
    {
        builder.Property(u => u.CampaignId).HasConversion(
            id => id.Value,
            dbValue => new CampaignId(dbValue)
        );

        builder.Property(u => u.Id).HasConversion(
            id => id.Value,
            dbValue => new UnitId(dbValue)
        );

        builder.Property(u => u.ArmyId).HasConversion(
            id => id.Value,
            dbValue => new ArmyId(dbValue)
        );

        builder.HasIndex(u => u.ArmyId);

        builder.Property(u => u.Type).HasConversion<string>();
        builder.Property(u => u.Faction).HasConversion<string>();
    }
}