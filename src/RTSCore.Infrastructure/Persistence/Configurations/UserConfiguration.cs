using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using RTSCore.Domain.Entities;

namespace RTSCore.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.HasIndex(u => u.Name).IsUnique();

        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(25);

        builder.Property(u => u.PasswordHash).IsRequired();
    }
}