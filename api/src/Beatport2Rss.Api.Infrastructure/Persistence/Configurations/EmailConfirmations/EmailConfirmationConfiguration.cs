using Beatport2Rss.Api.Domain.EmailConfirmations;
using Beatport2Rss.Api.Domain.Users;
using Beatport2Rss.Common.Domain.Common;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Api.Infrastructure.Persistence.Configurations.EmailConfirmations;

internal sealed class EmailConfirmationConfiguration : IEntityTypeConfiguration<EmailConfirmation>
{
    public void Configure(EntityTypeBuilder<EmailConfirmation> builder)
    {
        builder.ToTable(nameof(ApiDbContext.EmailConfirmations));
        builder.HasKey(confirmation => confirmation.Id);
        builder.Property(confirmation => confirmation.EmailAddress).HasMaxLength(EmailAddress.MaxLength).IsRequired();
        builder.Property(confirmation => confirmation.SecretHash).HasMaxLength(SecretHash.MaxLength).IsRequired();
        builder.HasIndex(confirmation => confirmation.SecretHash).IsUnique();
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(confirmation => confirmation.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
