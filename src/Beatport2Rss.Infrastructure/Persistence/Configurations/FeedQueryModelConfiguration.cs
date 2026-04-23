using Beatport2Rss.Infrastructure.Persistence.QueryModels;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Infrastructure.Persistence.Configurations;

internal sealed class FeedQueryModelConfiguration :
    IEntityTypeConfiguration<FeedQueryModel>
{
    public void Configure(EntityTypeBuilder<FeedQueryModel> builder)
    {
        builder.ToView("vwFeeds");

        builder.HasKey(feedQueryModel => feedQueryModel.Id);

        builder.Property(feedQueryModel => feedQueryModel.Id);
        builder.Property(feedQueryModel => feedQueryModel.CreatedAt);
        builder.Property(feedQueryModel => feedQueryModel.UserId);
        builder.Property(feedQueryModel => feedQueryModel.Name);
        builder.Property(feedQueryModel => feedQueryModel.Slug);
        builder.Property(feedQueryModel => feedQueryModel.IsActive);
        builder.Property(feedQueryModel => feedQueryModel.SubscriptionsCount);
    }
}