using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Releases;
using Beatport2Rss.Domain.Subscriptions;
using Beatport2Rss.Domain.Tracks;
using Beatport2Rss.Infrastructure.Persistence.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Infrastructure.Persistence.Configurations;

internal sealed class TrackConfiguration :
    IEntityTypeConfiguration<Track>
{
    public void Configure(EntityTypeBuilder<Track> builder)
    {
        builder.ToTable(nameof(Beatport2RssDbContext.Tracks));

        builder.HasKey(track => track.Id);

        builder.Property(track => track.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(track => track.CreatedAt)
            .IsRequired();

        builder.Property(track => track.BeatportId)
            .IsRequired();

        builder.Property(track => track.BeatportSlug)
            .HasMaxLength(BeatportSlug.MaxLength)
            .IsRequired();

        builder.Property(track => track.Number)
            .IsRequired();

        builder.Property(track => track.Name)
            .HasMaxLength(TrackName.MaxLength)
            .IsRequired();

        builder.Property(track => track.MixName)
            .HasMaxLength(MixName.MaxLength)
            .IsRequired();

        builder.Property(track => track.Length)
            .IsRequired();

        builder.Property(track => track.SampleUri)
            .IsUri();

        builder.Property(track => track.ReleaseId)
            .IsRequired();

        builder.OwnsMany(
            track => track.Subscriptions,
            navigationBuilder =>
            {
                navigationBuilder.ToTable(nameof(Beatport2RssDbContext.TrackSubscriptions));

                navigationBuilder.HasKey(trackSubscription => new { trackSubscription.TrackId, trackSubscription.SubscriptionId, trackSubscription.Type });

                navigationBuilder.Property(trackSubscription => trackSubscription.TrackId)
                    .IsRequired();

                navigationBuilder.Property(trackSubscription => trackSubscription.SubscriptionId)
                    .IsRequired();

                navigationBuilder.Property(trackSubscription => trackSubscription.Type)
                    .IsEnum();

                navigationBuilder
                    .WithOwner()
                    .HasForeignKey(trackSubscription => trackSubscription.TrackId);

                navigationBuilder
                    .HasOne<Subscription>()
                    .WithMany()
                    .HasForeignKey(trackSubscription => trackSubscription.SubscriptionId);

                navigationBuilder.HasIndex(trackSubscription => trackSubscription.SubscriptionId);
            });

        builder.HasOne<Release>()
            .WithMany(release => release.Tracks)
            .HasForeignKey(track => track.ReleaseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(track => track.BeatportId)
            .IsUnique();
    }
}