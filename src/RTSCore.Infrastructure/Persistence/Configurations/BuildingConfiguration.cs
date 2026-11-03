using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using RTSCore.Domain.Entities.Campaigns;
using RTSCore.Domain.Entities.Cities;

namespace RTSCore.Infrastructure.Persistence.Configurations;

public sealed class BuildingConfiguration : IEntityTypeConfiguration<Building>
{
    public void Configure(EntityTypeBuilder<Building> builder)
    {
        builder.Property(b => b.CampaignId).HasConversion(
            id => id.Value,
            dbValue => new CampaignId(dbValue)
        );

        builder.Property(b => b.Id).HasConversion(
            id => id.Value,
            dbValue => new BuildingId(dbValue)
        );

        builder.Property(b => b.CityId).HasConversion(
            id => id.Value,
            dbValue => new CityId(dbValue)
        );

        builder.HasIndex(b => b.CityId);

        builder.Property(b => b.Type).HasConversion<string>();
        builder.Property(b => b.Faction).HasConversion<string>();
    }
}