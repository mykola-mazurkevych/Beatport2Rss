using Beatport2Rss.Domain.Users;
using Beatport2Rss.Infrastructure.Persistence.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Infrastructure.Persistence.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(nameof(Beatport2RssDbContext.Users));

        builder.HasKey(user => user.Id);

        builder.Property(user => user.Id)
            .IsRequired();

        builder.Property(user => user.EmailAddress)
            .HasMaxLength(EmailAddress.MaxLength)
            .IsRequired();

        builder.Property(user => user.PasswordHash)
            .HasMaxLength(PasswordHash.MaxLength)
            .IsRequired();

        builder.Property(user => user.FirstName)
            .HasMaxLength(User.NameLength)
            .IsRequired(false);
        
        builder.Property(user => user.LastName)
            .HasMaxLength(User.NameLength)
            .IsRequired(false);

        builder.Property(user => user.Status)
            .IsEnum();

        builder.Property(user => user.CreatedDate)
            .IsRequired();

        builder.Navigation(user => user.Feeds)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(user => user.Tags)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(user => user.EmailAddress)
            .IsUnique();
    }
}