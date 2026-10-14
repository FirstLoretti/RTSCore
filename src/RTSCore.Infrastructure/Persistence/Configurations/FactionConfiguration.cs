using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using RTSCore.Domain.Entities.Campaign;
using RTSCore.Domain.Entities.Factions;
using RTSCore.Domain.ValueObjects.Identifiers;

namespace RTSCore.Infrastructure.Persistence.Configurations;

public class FactionConfiguration : IEntityTypeConfiguration<Faction>
{
    public void Configure(EntityTypeBuilder<Faction> builder)
    {
        builder.HasKey(f => f.Type);

        builder.Property(f => f.CampaignId).HasConversion(
            id => id.Value,
            value => new CampaignId(value)
        );

        builder.Property(f => f.CitiesCount)
            .HasComputedColumnSql

        builder.Property(f => f.PlayerType).HasConversion<string>();
        builder.Property(f => f.AiPersonalityType).HasConversion<string>();
        builder.Property(f => f.Type).HasConversion<string>();
        builder.Property(f => f.Gold).HasConversion<int>();
    }
}