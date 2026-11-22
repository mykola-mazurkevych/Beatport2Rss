using Beatport2Rss.Api.Domain.EmailVerifications;
using Beatport2Rss.Api.Domain.Users;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Api.Infrastructure.Persistence.Configurations.EmailVerifications;

internal sealed class EmailVerificationConfiguration : IEntityTypeConfiguration<EmailVerification>
{
    public void Configure(EntityTypeBuilder<EmailVerification> builder)
    {
        builder.ToTable(nameof(ApiDbContext.EmailVerifications));
        builder.HasKey(verification => verification.Id);
        builder.Property(verification => verification.EmailAddress).HasMaxLength(EmailAddress.MaxLength).IsRequired();
        builder.Property(verification => verification.SecretHash).HasMaxLength(EmailVerificationSecretHash.MaxLength).IsRequired();
        builder.HasIndex(verification => verification.SecretHash).IsUnique();
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(verification => verification.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
