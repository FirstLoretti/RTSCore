using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using RTSCore.Domain.AI;
using RTSCore.Domain.Entities.Campaigns;

namespace RTSCore.Infrastructure.Persistence.Configurations;

public sealed class AiAgentsConfiguration : IEntityTypeConfiguration<AiFactionAgent>
{
    public void Configure(EntityTypeBuilder<AiFactionAgent> builder)
    {
        builder.HasKey(a => new { a.CampaignId, a.FactionType });

        builder.Property(a => a.CampaignId).HasConversion(
            campaignId => campaignId.Value,
            dbValue => new CampaignId(dbValue)
        );
    }
}