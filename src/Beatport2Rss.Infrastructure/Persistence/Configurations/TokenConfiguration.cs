using Beatport2Rss.Domain.Tokens;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Infrastructure.Persistence.Configurations;

public sealed class TokenConfiguration : IEntityTypeConfiguration<Token>
{
    public void Configure(EntityTypeBuilder<Token> builder)
    {
        builder.ToTable(nameof(Beatport2RssDbContext.Tokens));

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .HasConversion(
                tokenId => tokenId.Value,
                value => TokenId.Create(value))
            .IsRequired();

        builder.Property(t => t.AccessToken)
            .HasConversion(
                accessToken => accessToken.Value,
                value => AccessToken.Create(value))
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(t => t.CreatedDate)
            .IsRequired();

        builder.Property(t => t.ExpirationDate)
            .IsRequired();

        builder.Ignore(t => t.IsExpired);
    }
}