using Beatport2Rss.Domain.Common;
using Beatport2Rss.Domain.Subscriptions;
using Beatport2Rss.Infrastructure.Persistence.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Infrastructure.Persistence.Configurations;

public sealed class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable(nameof(Beatport2RssDbContext.Subscriptions));

        builder.HasKey(subscription => subscription.Id);

        builder.Property(subscription => subscription.Id)
            .HasConversion(
                subscriptionId => subscriptionId.Value,
                value => SubscriptionId.Create(value))
            .IsRequired();

        builder.Property(subscription => subscription.BeatportType)
            .IsEnum();

        builder.Property(subscription => subscription.BeatportId)
            .HasConversion(
                beatportId => beatportId.Value,
                value => BeatportId.Create(value))
            .IsRequired();

        builder.Property(subscription => subscription.BeatportSlug)
            .HasConversion(
                beatportSlug => beatportSlug.Value,
                value => BeatportSlug.Create(value))
            .HasMaxLength(BeatportSlug.MaxLength)
            .IsRequired();

        builder.Property(subscription => subscription.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(subscription => subscription.ImageUri)
            .HasConversion(
                uri => uri.ToString(),
                uriString => new Uri(uriString))
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(subscription => subscription.CreatedDate)
            .IsRequired();

        builder.Property(subscription => subscription.PulledDate);

        builder.HasIndex(subscription => new { subscription.BeatportId, subscription.BeatportType })
            .IsUnique();

        builder.HasIndex(subscription => subscription.BeatportSlug);
    }
}