using Beatport2Rss.Collector.Domain.Artists;
using Beatport2Rss.Collector.Domain.Common.ValueObjects;
using Beatport2Rss.Collector.Domain.Releases;
using Beatport2Rss.Collector.Domain.Tracks;
using Beatport2Rss.Common.EntityFrameworkCore.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Collector.Infrastructure.Persistence.Configurations;

internal sealed class TrackConfiguration :
    IEntityTypeConfiguration<Track>
{
    public void Configure(EntityTypeBuilder<Track> builder)
    {
        builder.ToTable(nameof(CollectorDbContext.Tracks));

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
            track => track.Artists,
            navigationBuilder =>
            {
                navigationBuilder.ToTable(nameof(CollectorDbContext.TrackArtists));

                navigationBuilder.HasKey(trackArtist => new { trackArtist.TrackId, trackArtist.ArtistId, trackArtist.Type });

                navigationBuilder.Property(trackArtist => trackArtist.TrackId)
                    .IsRequired();

                navigationBuilder.Property(trackArtist => trackArtist.ArtistId)
                    .IsRequired();

                navigationBuilder.Property(trackArtist => trackArtist.Type)
                    .IsEnum();

                navigationBuilder
                    .WithOwner()
                    .HasForeignKey(trackArtist => trackArtist.TrackId);

                navigationBuilder.HasOne<Artist>()
                    .WithMany()
                    .HasForeignKey(trackArtist => trackArtist.ArtistId);

                navigationBuilder.HasIndex(trackArtist => trackArtist.ArtistId);
            });

        builder.HasOne<Release>()
            .WithMany(release => release.Tracks)
            .HasForeignKey(track => track.ReleaseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(track => track.BeatportId)
            .IsUnique();
    }
}