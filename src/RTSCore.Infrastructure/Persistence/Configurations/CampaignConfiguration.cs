using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using RTSCore.Domain.Entities.Campaigns;

namespace RTSCore.Infrastructure.Persistence.Configurations;

public sealed class CampaignConfiguration : IEntityTypeConfiguration<Campaign>
{
    public void Configure(EntityTypeBuilder<Campaign> builder)
    {
        builder.Property(c => c.Id).HasConversion(
            id => id.Value,
            dbId => new CampaignId(dbId)
        );

        builder.PrimitiveCollection(c => c.Factions)
            .ElementType(f => f.HasConversion<string>().HasMaxLength(256));
    }
}