using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using RTSCore.Application;

namespace RTSCore.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(u => u.Id).HasConversion(
            id => id.Value,
            dbValue => new UserId(dbValue)
        );

        builder.HasIndex(u => u.Id);
        builder.HasIndex(u => u.Name).IsUnique();
        builder.Property(u => u.PasswordHash).IsRequired();
    }
}