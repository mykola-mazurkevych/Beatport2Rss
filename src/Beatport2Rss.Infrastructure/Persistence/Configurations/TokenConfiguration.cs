using Beatport2Rss.Domain.Tokens;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Infrastructure.Persistence.Configurations;

public sealed class TokenConfiguration : IEntityTypeConfiguration<Token>
{
    public void Configure(EntityTypeBuilder<Token> builder)
    {
        builder.ToTable(nameof(Beatport2RssDbContext.Tokens));

        builder.HasKey(token => token.Id);

        builder.Property(token => token.Id)
            .IsRequired();

        builder.Property(token => token.AccessToken)
            .HasMaxLength(AccessToken.MaxLength)
            .IsRequired();

        builder.Property(token => token.CreatedDate)
            .IsRequired();

        builder.Property(token => token.ExpirationDate)
            .IsRequired();

        builder.Ignore(token => token.IsExpired);
    }
}