using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using RTSCore.Domain.Entities.Campaigns;
using RTSCore.Domain.Entities.Diplomacy;

namespace RTSCore.Infrastructure.Persistence.Configurations;

public sealed class DiplomacyRelationConfiguration() : IEntityTypeConfiguration<DiplomacyRelation>
{
    public void Configure(EntityTypeBuilder<DiplomacyRelation> builder)
    {
        builder.Property(c => c.CampaignId).HasConversion(
            id => id.Value,
            dbValue => new CampaignId(dbValue)
        );

        builder.Property(c => c.Id).HasConversion(
            id => id.Value,
            dbValue => new DiplomacyRelationId(dbValue)
        );

        builder.HasIndex(r => new { r.CampaignId, r.FactionA, r.FactionB }).IsUnique();

        builder.Property(r => r.FactionA).HasConversion<string>();
        builder.Property(r => r.FactionB).HasConversion<string>();
    }
}