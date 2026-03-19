using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Subscriptions;
using Beatport2Rss.Domain.Tags;
using Beatport2Rss.Infrastructure.Persistence.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Infrastructure.Persistence.Configurations;

internal sealed class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable(nameof(Beatport2RssDbContext.Subscriptions));

        builder.HasKey(subscription => subscription.Id);

        builder.Property(subscription => subscription.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(subscription => subscription.CreatedAt)
            .IsRequired();

        builder.Property(subscription => subscription.BeatportType)
            .IsEnum();

        builder.Property(subscription => subscription.BeatportId)
            .IsRequired();

        builder.Property(subscription => subscription.BeatportSlug)
            .HasMaxLength(BeatportSlug.MaxLength)
            .IsRequired();

        builder.Property(subscription => subscription.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(subscription => subscription.ImageUri)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(subscription => subscription.RefreshedAt)
            .IsRequired(false);

        builder.OwnsMany(
            subscription => subscription.Tags,
            navigationBuilder =>
            {
                const string subscriptionIdPropertyName = "SubscriptionId";

                navigationBuilder.ToTable(nameof(Beatport2RssDbContext.SubscriptionTags));

                navigationBuilder.HasKey(subscriptionIdPropertyName, nameof(SubscriptionTag.TagId));

                navigationBuilder.WithOwner()
                    .HasForeignKey(subscriptionIdPropertyName);

                navigationBuilder.Property(subscriptionTag => subscriptionTag.TagId)
                    .IsRequired();

                navigationBuilder.HasOne<Tag>()
                    .WithMany()
                    .HasForeignKey(subscriptionTag => subscriptionTag.TagId);
            });

        builder.HasIndex(subscription => new { subscription.BeatportType, subscription.BeatportId, subscription.BeatportSlug })
            .IsUnique();

        builder.HasIndex(subscription => subscription.BeatportId);
        builder.HasIndex(subscription => subscription.BeatportSlug);
    }
}