using Beatport2Rss.Builder.Domain.Subscriptions;
using Beatport2Rss.Builder.Domain.Tags;
using Beatport2Rss.Common.Domain.Subscriptions;
using Beatport2Rss.Common.EntityFrameworkCore.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Builder.Infrastructure.Persistence.Configurations;

internal sealed class SubscriptionConfiguration :
    IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable(nameof(BuilderDbContext.Subscriptions));

        builder.HasKey(subscription => subscription.Id);

        builder.Property(subscription => subscription.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(subscription => subscription.CreatedAt)
            .IsRequired();

        builder.Property(subscription => subscription.Type)
            .IsEnum();

        builder.Property(subscription => subscription.Name)
            .HasMaxLength(SubscriptionName.MaxLength)
            .IsRequired();

        builder.Property(subscription => subscription.BeatportId)
            .IsRequired();

        builder.HasIndex(subscription => new { subscription.Type, subscription.BeatportId })
            .IsUnique();

        builder.HasIndex(subscription => subscription.BeatportId);

        builder.OwnsMany(
            subscription => subscription.Tags,
            navigationBuilder =>
            {
                navigationBuilder.ToTable(nameof(BuilderDbContext.SubscriptionTags));

                navigationBuilder.HasKey(subscriptionTag => new { subscriptionTag.SubscriptionId, subscriptionTag.TagId });

                navigationBuilder.Property(subscriptionTag => subscriptionTag.SubscriptionId)
                    .IsRequired();

                navigationBuilder.Property(subscriptionTag => subscriptionTag.TagId)
                    .IsRequired();

                navigationBuilder
                    .WithOwner()
                    .HasForeignKey(subscriptionTag => subscriptionTag.SubscriptionId);

                navigationBuilder
                    .HasOne<Tag>()
                    .WithMany()
                    .HasForeignKey(subscriptionTag => subscriptionTag.TagId)
                    .OnDelete(DeleteBehavior.Cascade);

                navigationBuilder.HasIndex(subscriptionTag => subscriptionTag.TagId);
            });
    }
}