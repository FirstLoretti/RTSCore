using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using RTSCore.Domain.Entities;

namespace RTSCore.Infrastructure.Persistence.Configurations;

public class FactionConfiguration : IEntityTypeConfiguration<Faction>
{
    public void Configure(EntityTypeBuilder<Faction> builder)
    {
        builder.HasKey(p => p.Type);

        builder.Property(p => p.Type).HasConversion<string>();
        builder.Property(p => p.Gold).HasConversion<int>();
    }
}