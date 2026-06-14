using Beatport2Rss.Common.EntityFrameworkCore.Extensions;
using Beatport2Rss.ReleaseCollector.Domain.Common.ValueObjects;
using Beatport2Rss.ReleaseCollector.Domain.Subscriptions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.ReleaseCollector.Infrastructure.Persistence.Configurations;

internal sealed class SubscriptionConfiguration :
    IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable(nameof(ReleaseCollectorDbContext.Subscriptions));

        builder.HasKey(subscription => subscription.Id);

        builder.Property(subscription => subscription.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(subscription => subscription.CreatedAt)
            .IsRequired();

        builder.Property(subscription => subscription.Name)
            .HasMaxLength(SubscriptionName.MaxLength)
            .IsRequired();

        builder.Property(subscription => subscription.BeatportType)
            .IsEnum();

        builder.Property(subscription => subscription.BeatportId)
            .IsRequired();

        builder.Property(subscription => subscription.BeatportSlug)
            .HasMaxLength(BeatportSlug.MaxLength)
            .IsRequired();

        builder.HasIndex(subscription => new { subscription.BeatportType, subscription.BeatportId })
            .IsUnique();

        builder.HasIndex(subscription => subscription.BeatportSlug);
    }
}