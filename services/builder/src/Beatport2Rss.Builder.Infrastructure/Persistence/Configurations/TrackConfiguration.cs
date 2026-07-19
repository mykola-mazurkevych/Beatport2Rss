using Beatport2Rss.Builder.Domain.Artists;
using Beatport2Rss.Builder.Domain.Releases;
using Beatport2Rss.Builder.Domain.Tracks;
using Beatport2Rss.Common.EntityFrameworkCore.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Builder.Infrastructure.Persistence.Configurations;

internal sealed class TrackConfiguration :
    IEntityTypeConfiguration<Track>
{
    public void Configure(EntityTypeBuilder<Track> builder)
    {
        builder.ToTable(nameof(BuilderDbContext.Tracks));

        builder.HasKey(track => track.Id);

        builder.Property(track => track.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(track => track.CreatedAt)
            .IsRequired();

        builder.Property(track => track.BeatportId)
            .IsRequired();

        builder.Property(release => release.BeatportUri)
            .IsUri();

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
                navigationBuilder.ToTable(nameof(BuilderDbContext.TrackArtists));

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