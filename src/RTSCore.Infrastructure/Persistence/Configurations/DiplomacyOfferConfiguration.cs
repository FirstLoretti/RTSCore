using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using RTSCore.Domain;
using RTSCore.Domain.Entities.Campaigns;
using RTSCore.Domain.Entities.Diplomacy;

namespace RTSCore.Infrastructure.Persistence.Configurations;

public sealed class DiplomacyOfferConfiguration : IEntityTypeConfiguration<DiplomacyOffer>
{
    public void Configure(EntityTypeBuilder<DiplomacyOffer> builder)
    {
        builder.Property(c => c.CampaignId).HasConversion(
            id => id.Value,
            dbValue => new CampaignId(dbValue)
        );

        builder.Property(c => c.Id).HasConversion(
            id => id.Value,
            dbValue => new DiplomacyOfferId(dbValue)
        );

        builder.HasIndex(o => new { o.CampaignId, o.Status, o.Target });
    }
}