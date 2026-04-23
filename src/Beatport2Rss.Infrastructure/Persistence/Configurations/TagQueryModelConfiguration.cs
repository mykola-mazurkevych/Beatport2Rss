using Beatport2Rss.Infrastructure.Persistence.QueryModels;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Infrastructure.Persistence.Configurations;

internal sealed class TagQueryModelConfiguration :
    IEntityTypeConfiguration<TagQueryModel>
{
    public void Configure(EntityTypeBuilder<TagQueryModel> builder)
    {
        builder.ToView("vwTags");

        builder.HasKey(tagQueryModel => tagQueryModel.Id);

        builder.Property(tagQueryModel => tagQueryModel.Id);
        builder.Property(tagQueryModel => tagQueryModel.CreatedAt);
        builder.Property(tagQueryModel => tagQueryModel.UserId);
        builder.Property(tagQueryModel => tagQueryModel.Name);
        builder.Property(tagQueryModel => tagQueryModel.Slug);
    }
}