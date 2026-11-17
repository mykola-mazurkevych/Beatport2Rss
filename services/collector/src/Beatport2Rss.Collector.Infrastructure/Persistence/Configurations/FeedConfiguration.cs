using Beatport2Rss.Collector.Domain.Feeds;
using Beatport2Rss.Collector.Domain.Subscriptions;
using Beatport2Rss.Collector.Domain.Users;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Collector.Infrastructure.Persistence.Configurations;

internal sealed class FeedConfiguration :
    IEntityTypeConfiguration<Feed>
{
    public void Configure(EntityTypeBuilder<Feed> builder)
    {
        builder.ToTable(nameof(CollectorDbContext.Feeds));

        builder.HasKey(feed => feed.Id);

        builder.Property(feed => feed.Id)
            .ValueGeneratedNever();

        builder.Property(feed => feed.CreatedAt)
            .IsRequired();

        builder.Property(feed => feed.UserId)
            .IsRequired();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(feed => feed.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.OwnsMany(
            feed => feed.Subscriptions,
            navigationBuilder =>
            {
                navigationBuilder.ToTable(nameof(CollectorDbContext.FeedSubscriptions));

                navigationBuilder.HasKey(feedSubscription => new { feedSubscription.FeedId, feedSubscription.SubscriptionId });

                navigationBuilder.Property(feedSubscription => feedSubscription.FeedId)
                    .IsRequired();

                navigationBuilder.Property(feedSubscription => feedSubscription.SubscriptionId)
                    .IsRequired();

                navigationBuilder
                    .WithOwner()
                    .HasForeignKey(feedSubscription => feedSubscription.FeedId);

                navigationBuilder.HasOne<Subscription>()
                    .WithMany()
                    .HasForeignKey(feedSubscription => feedSubscription.SubscriptionId);

                navigationBuilder.HasIndex(feedSubscription => feedSubscription.SubscriptionId);
            });
    }
}