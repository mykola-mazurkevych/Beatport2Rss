using Beatport2Rss.Domain.Users;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(nameof(Beatport2RssDbContext.Users));

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasConversion(
                userId => userId.Value,
                value => UserId.Create(value))
            .IsRequired();

        builder.Property(u => u.Username)
            .HasConversion(
                username => username.Value,
                value => Username.Create(value))
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(u => u.Slug)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(u => u.PasswordHash)
            .HasConversion(
                passwordHash => passwordHash.Value,
                value => PasswordHash.Create(value))
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.EmailAddress)
            .HasConversion(
                email => email.Value,
                value => EmailAddress.Create(value))
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.CreatedDate)
            .IsRequired();

        builder.Navigation(u => u.Feeds)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(u => u.Tags)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(u => u.Username)
            .IsUnique();

        builder.HasIndex(u => u.Slug)
            .IsUnique();

        builder.HasIndex(u => u.EmailAddress)
            .IsUnique();
    }
}