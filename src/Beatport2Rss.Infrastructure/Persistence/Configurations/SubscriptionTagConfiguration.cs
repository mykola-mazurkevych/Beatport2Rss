using Beatport2Rss.Domain.Subscriptions;
using Beatport2Rss.Domain.Tags;
using Beatport2Rss.Infrastructure.Persistence.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Infrastructure.Persistence.Configurations;

internal sealed class SubscriptionTagConfiguration : IEntityTypeConfiguration<SubscriptionTag>
{
    public void Configure(EntityTypeBuilder<SubscriptionTag> builder)
    {
        builder.ToTable(nameof(Beatport2RssDbContext.SubscriptionTags));

        builder.HasKey(subscriptionTag => new { subscriptionTag.SubscriptionId, subscriptionTag.TagId });

        builder.Property(subscriptionTag => subscriptionTag.SubscriptionId)
            .IsRequired();

        builder.Property(subscriptionTag => subscriptionTag.TagId)
            .IsRequired();

        builder.Property(subscriptionTag => subscriptionTag.CreatedAt)
            .IsRequired();

        builder.HasOne<Subscription>()
            .WithMany()
            .HasForeignKey(subscriptionTag => subscriptionTag.SubscriptionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Tag>()
            .WithMany()
            .HasForeignKey(subscriptionTag => subscriptionTag.TagId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}