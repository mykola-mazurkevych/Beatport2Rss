using Beatport2Rss.Domain.Common;
using Beatport2Rss.Domain.Users;
using Beatport2Rss.Infrastructure.Persistence.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(nameof(Beatport2RssDbContext.Users));

        builder.HasKey(user => user.Id);

        builder.Property(user => user.Id)
            .HasConversion(
                userId => userId.Value,
                value => UserId.Create(value))
            .IsRequired();

        builder.Property(user => user.Username)
            .HasConversion(
                username => username.Value,
                value => Username.Create(value))
            .HasMaxLength(Username.MaxLength)
            .IsRequired();

        builder.Property(user => user.Slug)
            .HasConversion(
                slug => slug.Value,
                value => Slug.Create(value))
            .HasMaxLength(Slug.MaxLength)
            .IsRequired();
        
        builder.Property(user => user.EmailAddress)
            .HasConversion(
                emailAddress => emailAddress.Value,
                value => EmailAddress.Create(value))
            .HasMaxLength(EmailAddress.MaxLength)
            .IsRequired();

        builder.Property(user => user.PasswordHash)
            .HasConversion(
                passwordHash => passwordHash.Value,
                value => PasswordHash.Create(value))
            .HasMaxLength(PasswordHash.MaxLength)
            .IsRequired();

        builder.Property(user => user.Status)
            .IsEnum();

        builder.Property(user => user.CreatedDate)
            .IsRequired();

        builder.Navigation(user => user.Feeds)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(user => user.Tags)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(user => user.Username)
            .IsUnique();

        builder.HasIndex(user => user.Slug)
            .IsUnique();

        builder.HasIndex(user => user.EmailAddress)
            .IsUnique();
    }
}