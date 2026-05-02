using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Countries;
using Beatport2Rss.Domain.Subscriptions;
using Beatport2Rss.Domain.Tags;
using Beatport2Rss.Infrastructure.Persistence.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Infrastructure.Persistence.Configurations;

internal sealed class SubscriptionConfiguration :
    IEntityTypeConfiguration<Subscription>
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

        builder.Property(subscription => subscription.Name)
            .HasMaxLength(SubscriptionName.MaxLength)
            .IsRequired();

        builder.Property(subscription => subscription.Slug)
            .HasMaxLength(Slug.MaxLength)
            .IsRequired();

        builder.Property(subscription => subscription.BeatportType)
            .IsEnum();

        builder.Property(subscription => subscription.BeatportId)
            .IsRequired();

        builder.Property(subscription => subscription.BeatportSlug)
            .HasMaxLength(BeatportSlug.MaxLength)
            .IsRequired();

        builder.Property(subscription => subscription.ImageUri)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(subscription => subscription.RefreshedAt)
            .IsRequired(false);

        builder.Property(subscription => subscription.CountryCode)
            .IsRequired(false);
        
        builder.HasOne<Country>()
            .WithMany()
            .HasForeignKey(subscription => subscription.CountryCode);

        builder.OwnsMany(
            subscription => subscription.Tags,
            navigationBuilder =>
            {
                navigationBuilder.ToTable(nameof(Beatport2RssDbContext.SubscriptionTags));

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
                    .HasForeignKey(subscriptionTag => subscriptionTag.TagId);

                navigationBuilder.HasIndex(subscriptionTag => subscriptionTag.TagId);
            });

        builder
            .HasIndex(subscription => subscription.Slug)
            .IsUnique();

        builder
            .HasIndex(subscription => new { subscription.BeatportType, subscription.BeatportId })
            .IsUnique();
    }
}