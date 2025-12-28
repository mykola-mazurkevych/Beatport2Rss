using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Subscriptions;
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

        builder.Property(subscription => subscription.CreatedDate)
            .IsRequired();

        builder.Property(subscription => subscription.PulledDate);

        builder.HasIndex(subscription => new { subscription.BeatportId, subscription.BeatportType })
            .IsUnique();

        builder.HasIndex(subscription => subscription.BeatportSlug);
    }
}