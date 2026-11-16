using Beatport2Rss.Builder.Domain.Feeds;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Builder.Infrastructure.Persistence.Configurations;

internal sealed class RssFeedDocumentConfiguration :
    IEntityTypeConfiguration<RssFeedDocument>
{
    public void Configure(EntityTypeBuilder<RssFeedDocument> builder)
    {
        builder.ToTable(nameof(BuilderDbContext.RssFeedDocuments));

        builder.HasKey(rssFeedDocument => rssFeedDocument.Id);

        builder.Property(rssFeedDocument => rssFeedDocument.Id)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(rssFeedDocument => rssFeedDocument.CreatedAt)
            .IsRequired();

        builder.Property(rssFeedDocument => rssFeedDocument.GeneratedAt)
            .IsRequired();

        builder.Property(rssFeedDocument => rssFeedDocument.Content)
            .HasColumnType("text")
            .IsRequired();
    }
}