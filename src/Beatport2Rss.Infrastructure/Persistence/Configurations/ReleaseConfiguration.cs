using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Releases;
using Beatport2Rss.Domain.Subscriptions;
using Beatport2Rss.Infrastructure.Persistence.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Infrastructure.Persistence.Configurations;

internal sealed class ReleaseConfiguration :
    IEntityTypeConfiguration<Release>
{
    public void Configure(EntityTypeBuilder<Release> builder)
    {
        builder.ToTable(nameof(Beatport2RssDbContext.Releases));

        builder.HasKey(release => release.Id);

        builder.Property(release => release.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(release => release.CreatedAt)
            .IsRequired();

        builder.Property(release => release.BeatportId)
            .IsRequired();

        builder.Property(release => release.BeatportSlug)
            .HasMaxLength(BeatportSlug.MaxLength)
            .IsRequired();

        builder.Property(release => release.Name)
            .HasMaxLength(ReleaseName.MaxLength)
            .IsRequired();

        builder.Property(release => release.CatalogNumber)
            .HasMaxLength(CatelogNumber.MaxLength)
            .IsRequired();

        builder.Property(release => release.ImageUri)
            .IsUri();

        builder.Property(release => release.ReleaseDate)
            .IsRequired();

        builder.Property(release => release.TracksCount)
            .IsRequired();

        builder.Property(release => release.Status)
            .IsEnum();

        builder.OwnsMany(
            release => release.Subscriptions,
            navigationBuilder =>
            {
                navigationBuilder.ToTable(nameof(Beatport2RssDbContext.ReleaseSubscriptions));

                navigationBuilder.HasKey(releaseSubscription => new { releaseSubscription.ReleaseId, releaseSubscription.SubscriptionId, releaseSubscription.Type });

                navigationBuilder.Property(releaseSubscription => releaseSubscription.ReleaseId)
                    .IsRequired();

                navigationBuilder.Property(releaseSubscription => releaseSubscription.SubscriptionId)
                    .IsRequired();

                navigationBuilder.Property(releaseSubscription => releaseSubscription.Type)
                    .IsEnum();

                navigationBuilder
                    .WithOwner()
                    .HasForeignKey(releaseSubscription => releaseSubscription.ReleaseId);

                navigationBuilder
                    .HasOne<Subscription>()
                    .WithMany()
                    .HasForeignKey(releaseSubscription => releaseSubscription.SubscriptionId);

                navigationBuilder.HasIndex(releaseSubscription => releaseSubscription.SubscriptionId);
            });

        builder.Navigation(release => release.Tracks)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(release => release.BeatportId)
            .IsUnique();
    }
}