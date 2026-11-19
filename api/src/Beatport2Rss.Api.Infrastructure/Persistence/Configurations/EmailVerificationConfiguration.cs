using Beatport2Rss.Api.Application.UseCases.EmailConfirmations;
using Beatport2Rss.Api.Domain.Users;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Api.Infrastructure.Persistence.Configurations;

internal sealed class EmailVerificationConfiguration : IEntityTypeConfiguration<EmailVerification>
{
    public void Configure(EntityTypeBuilder<EmailVerification> builder)
    {
        builder.ToTable(nameof(ApiDbContext.EmailVerifications));
        builder.HasKey(verification => verification.Id);
        builder.Property(verification => verification.EmailAddress).HasMaxLength(EmailAddress.MaxLength).IsRequired();
        builder.Property(verification => verification.SecretHash).HasMaxLength(EmailConfirmationIdentifier.HashLength).IsRequired();
        builder.HasIndex(verification => verification.SecretHash).IsUnique();
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(verification => verification.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
