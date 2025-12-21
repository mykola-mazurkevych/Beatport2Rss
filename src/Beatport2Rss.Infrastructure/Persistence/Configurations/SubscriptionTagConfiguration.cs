using Beatport2Rss.Domain.Subscriptions;
using Beatport2Rss.Domain.Tags;
using Beatport2Rss.Infrastructure.Persistence.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Infrastructure.Persistence.Configurations;

public sealed class SubscriptionTagConfiguration : IEntityTypeConfiguration<SubscriptionTag>
{
    public void Configure(EntityTypeBuilder<SubscriptionTag> builder)
    {
        builder.ToTable(nameof(Beatport2RssDbContext.SubscriptionTags));

        builder.HasKey(subscriptionTag => new { subscriptionTag.SubscriptionId, subscriptionTag.TagId });

        builder.Property(subscriptionTag => subscriptionTag.SubscriptionId)
            .HasConversion(
                subscriptionId => subscriptionId.Value,
                value => SubscriptionId.Create(value))
            .IsRequired();

        builder.Property(subscriptionTag => subscriptionTag.TagId)
            .HasConversion(
                tagId => tagId.Value,
                value => TagId.Create(value))
            .IsRequired();

        builder.Property(subscriptionTag => subscriptionTag.CreatedDate)
            .IsRequired()
            .HasDefaultValueSql("current_timestamp");

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