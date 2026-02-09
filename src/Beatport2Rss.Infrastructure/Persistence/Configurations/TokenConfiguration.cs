using Beatport2Rss.Domain.Tokens;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Infrastructure.Persistence.Configurations;

internal sealed class TokenConfiguration : IEntityTypeConfiguration<Token>
{
    public void Configure(EntityTypeBuilder<Token> builder)
    {
        builder.ToTable(nameof(Beatport2RssDbContext.Tokens));

        builder.HasKey(token => token.Id);

        builder.Property(token => token.Id)
            .IsRequired();

        builder.Property(token => token.CreatedAt)
            .IsRequired();

        builder.Property(token => token.AccessToken)
            .HasMaxLength(BeatportAccessToken.MaxLength)
            .IsRequired();

        builder.Property(token => token.ExpiresAt)
            .IsRequired();
    }
}