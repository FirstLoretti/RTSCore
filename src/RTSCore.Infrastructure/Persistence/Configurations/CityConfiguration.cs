using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using RTSCore.Domain.Entities.Campaigns;
using RTSCore.Domain.Entities.Cities;

namespace RTSCore.Infrastructure.Persistence.Configurations;

public sealed class CityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.Property(c => c.CampaignId).HasConversion(
            id => id.Value,
            dbValue => new CampaignId(dbValue)
        );

        builder.Property(c => c.Id).HasConversion(
            id => id.Value,
            dbValue => new CityId(dbValue)
        );

        builder.ComplexProperty(c => c.Coordinates);

        builder.Property(c => c.Type).HasConversion<string>();
        builder.Property(c => c.Faction).HasConversion<string>();

        builder.HasMany(c => c.Buildings)
            .WithOne()
            .HasForeignKey(b => b.CityId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}