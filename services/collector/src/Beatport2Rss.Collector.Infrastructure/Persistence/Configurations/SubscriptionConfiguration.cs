using Beatport2Rss.Collector.Domain.Subscriptions;
using Beatport2Rss.Common.EntityFrameworkCore.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Collector.Infrastructure.Persistence.Configurations;

internal sealed class SubscriptionConfiguration :
    IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable(nameof(CollectorDbContext.Subscriptions));

        builder.HasKey(subscription => subscription.Id);

        builder.Property(subscription => subscription.Id)
            .IsRequired();

        builder.Property(subscription => subscription.CreatedAt)
            .IsRequired();

        builder.Property(subscription => subscription.Type)
            .IsEnum();

        builder.Property(subscription => subscription.BeatportId)
            .IsRequired();

        builder.Property(subscription => subscription.BeatportSlug)
            .HasMaxLength(BeatportSlug.MaxLength)
            .IsRequired();

        builder.Property(subscription => subscription.SubscribersCount)
            .IsRequired();

        builder.Property(subscription => subscription.RefreshedAt)
            .IsRequired(required: false);

        builder.HasIndex(subscription => new { subscription.Type, subscription.BeatportId })
            .IsUnique();
    }
}