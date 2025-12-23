using Beatport2Rss.Domain.Common.ValueObjects;
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
            .IsRequired();

        builder.Property(user => user.Username)
            .HasMaxLength(Username.MaxLength)
            .IsRequired();

        builder.Property(user => user.Slug)
            .HasMaxLength(Slug.MaxLength)
            .IsRequired();
        
        builder.Property(user => user.EmailAddress)
            .HasMaxLength(EmailAddress.MaxLength)
            .IsRequired();

        builder.Property(user => user.PasswordHash)
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