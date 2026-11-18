using Beatport2Rss.Builder.Infrastructure.Persistence.QueryModels;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Builder.Infrastructure.Persistence.Configurations;

internal sealed class RssFeedDocumentQueryModelConfiguration :
    IEntityTypeConfiguration<RssFeedDocumentQueryModel>
{
    public void Configure(EntityTypeBuilder<RssFeedDocumentQueryModel> builder)
    {
        builder.ToView("vwRssFeedDocuments");

        builder.HasKey(queryModel => queryModel.Id);

        builder.Property(queryModel => queryModel.Id);
        builder.Property(queryModel => queryModel.Slug);
        builder.Property(queryModel => queryModel.GeneratedAt);
        builder.Property(queryModel => queryModel.Content);
    }
}