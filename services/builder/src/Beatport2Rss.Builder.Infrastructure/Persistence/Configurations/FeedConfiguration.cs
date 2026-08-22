using Beatport2Rss.Builder.Domain.Feeds;
using Beatport2Rss.Builder.Domain.Subscriptions;
using Beatport2Rss.Common.EntityFrameworkCore.Extensions;
using Beatport2Rss.Common.SharedKernel.ValueObjects;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Builder.Infrastructure.Persistence.Configurations;

internal sealed class FeedConfiguration :
    IEntityTypeConfiguration<Feed>
{
    public void Configure(EntityTypeBuilder<Feed> builder)
    {
        builder.ToTable(nameof(BuilderDbContext.Feeds));

        builder.HasKey(feed => feed.Id);

        builder.Property(feed => feed.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(feed => feed.CreatedAt)
            .IsRequired();

        builder.Property(feed => feed.Name)
            .HasMaxLength(FeedName.MaxLength)
            .IsRequired();

        builder.Property(feed => feed.Slug)
            .HasMaxLength(Slug.MaxLength)
            .IsRequired();

        builder.Property(feed => feed.AuthorName)
            .HasMaxLength(AuthorName.MaxLength)
            .IsRequired(false);

        builder.Property(feed => feed.Status)
            .IsEnum();

        builder.OwnsMany(
            feed => feed.Subscriptions,
            navigationBuilder =>
            {
                navigationBuilder.ToTable("FeedSubscriptions");

                navigationBuilder.HasKey(feedSubscription => new { feedSubscription.FeedId, feedSubscription.SubscriptionId });

                navigationBuilder.Property(feedSubscription => feedSubscription.FeedId)
                    .IsRequired();

                navigationBuilder.Property(feedSubscription => feedSubscription.SubscriptionId)
                    .IsRequired();

                navigationBuilder
                    .WithOwner()
                    .HasForeignKey(feedSubscription => feedSubscription.FeedId);

                navigationBuilder
                    .HasOne<Subscription>()
                    .WithMany()
                    .HasForeignKey(feedSubscription => feedSubscription.SubscriptionId);

                navigationBuilder.HasIndex(feedSubscription => feedSubscription.SubscriptionId);
            });
    }
}