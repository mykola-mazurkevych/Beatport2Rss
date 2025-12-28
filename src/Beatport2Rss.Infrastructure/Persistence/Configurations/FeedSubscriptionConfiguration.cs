using Beatport2Rss.Domain.Feeds;
using Beatport2Rss.Domain.Subscriptions;
using Beatport2Rss.Infrastructure.Persistence.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Infrastructure.Persistence.Configurations;

internal sealed class FeedSubscriptionConfiguration : IEntityTypeConfiguration<FeedSubscription>
{
    public void Configure(EntityTypeBuilder<FeedSubscription> builder)
    {
        builder.ToTable(nameof(Beatport2RssDbContext.FeedSubscriptions));

        builder.HasKey(feedSubscription => new { feedSubscription.FeedId, feedSubscription.SubscriptionId });

        builder.Property(feedSubscription => feedSubscription.FeedId)
            .IsRequired();

        builder.Property(feedSubscription => feedSubscription.SubscriptionId)
            .IsRequired();

        builder.Property(feedSubscription => feedSubscription.CreatedDate)
            .IsRequired()
            .HasDefaultValueSql("current_timestamp");

        builder.HasOne<Feed>()
            .WithMany()
            .HasForeignKey(feedSubscription => feedSubscription.FeedId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Subscription>()
            .WithMany()
            .HasForeignKey(feedSubscription => feedSubscription.SubscriptionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}