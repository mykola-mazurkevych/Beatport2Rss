using Beatport2Rss.Domain.Subscriptions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Infrastructure.Persistence.Configurations;

public sealed class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable(nameof(Beatport2RssDbContext.Subscriptions));

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasConversion(
                subscriptionId => subscriptionId.Value,
                value => SubscriptionId.Create(value))
            .IsRequired();

        builder.Property(s => s.BeatportType)
            .HasConversion<EnumToStringConverter<BeatportEntityType>>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(s => s.BeatportId)
            .IsRequired();

        builder.Property(s => s.BeatportSlug)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(s => s.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(s => s.ImageUri)
            .HasConversion(
                uri => uri.ToString(),
                value => new Uri(value))
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(s => s.CreatedDate)
            .IsRequired();

        builder.Property(s => s.PulledDate);

        builder.HasIndex(s => new { s.BeatportId, s.BeatportType })
            .IsUnique();

        builder.HasIndex(s => s.BeatportSlug);
    }
}
