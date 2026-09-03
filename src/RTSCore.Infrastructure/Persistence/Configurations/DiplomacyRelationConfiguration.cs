using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using RTSCore.Domain.Entities;

namespace RTSCore.Infrastructure.Persistence.Configurations;

public class DiplomacyRelationConfiguration() : IEntityTypeConfiguration<DiplomacyRelation>
{
    public void Configure(EntityTypeBuilder<DiplomacyRelation> builder)
    {
        builder.HasKey(r => r.Id);

        builder.HasIndex(r => new { r.FactionA, r.FactionB }).IsUnique();

        builder.Property(r => r.FactionA).HasConversion<string>().IsRequired();
        builder.Property(r => r.FactionB).HasConversion<string>().IsRequired();
    }
}