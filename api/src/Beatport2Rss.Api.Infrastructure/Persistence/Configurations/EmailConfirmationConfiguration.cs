using Beatport2Rss.Api.Domain.EmailConfirmations;
using Beatport2Rss.Api.Domain.Users;
using Beatport2Rss.Common.Domain.Common;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Api.Infrastructure.Persistence.Configurations;

internal sealed class EmailConfirmationConfiguration :
    IEntityTypeConfiguration<EmailConfirmation>
{
    public void Configure(EntityTypeBuilder<EmailConfirmation> builder)
    {
        builder.ToTable(nameof(ApiDbContext.EmailConfirmations));

        builder.HasKey(emailConfirmation => emailConfirmation.Id);

        builder.Property(emailConfirmation => emailConfirmation.Id)
            .IsRequired();
        
        builder.Property(emailConfirmation => emailConfirmation.CreatedAt)
            .IsRequired();
        
        builder.Property(emailConfirmation => emailConfirmation.UserId)
            .IsRequired();

        builder.Property(emailConfirmation => emailConfirmation.EmailAddress)
            .HasMaxLength(EmailAddress.MaxLength)
            .IsRequired();

        builder.Property(emailConfirmation => emailConfirmation.ExpiresAt)
            .IsRequired();

        builder.Property(emailConfirmation => emailConfirmation.ConfirmedAt)
            .IsRequired(required: false);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(emailConfirmation => emailConfirmation.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}