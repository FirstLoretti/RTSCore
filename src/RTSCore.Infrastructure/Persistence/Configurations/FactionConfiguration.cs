using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using RTSCore.Domain.Entities.Campaigns;
using RTSCore.Domain.Entities.Factions;

namespace RTSCore.Infrastructure.Persistence.Configurations;

public sealed class FactionConfiguration : IEntityTypeConfiguration<Faction>
{
    public void Configure(EntityTypeBuilder<Faction> builder)
    {
        builder.HasKey(f => new { f.CampaignId, f.Type });

        builder.Property(f => f.CampaignId).HasConversion(
            id => id.Value,
            value => new CampaignId(value)
        );

        builder.Property(f => f.Type).HasConversion<string>();
    }
}