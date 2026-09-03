using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using RTSCore.Domain.Entities;

namespace RTSCore.Infrastructure.Persistence.Configurations;

public class DiplomacyOfferConfiguration : IEntityTypeConfiguration<DiplomacyOffer>
{
    public void Configure(EntityTypeBuilder<DiplomacyOffer> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Initiator).HasConversion<string>().IsRequired();
        builder.Property(o => o.Target).HasConversion<string>().IsRequired();
        builder.Property(o => o.Type).HasConversion<string>().IsRequired();
        builder.Property(o => o.Status).HasConversion<string>().IsRequired();
    }
}