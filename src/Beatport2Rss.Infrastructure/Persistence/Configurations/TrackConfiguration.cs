using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Releases;
using Beatport2Rss.Domain.Tracks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Infrastructure.Persistence.Configurations;

internal sealed class TrackConfiguration : IEntityTypeConfiguration<Track>
{
    public void Configure(EntityTypeBuilder<Track> builder)
    {
        builder.ToTable(nameof(Beatport2RssDbContext.Tracks));

        builder.HasKey(track => track.Id);

        builder.Property(track => track.Id)
            .IsRequired();

        builder.Property(track => track.BeatportId)
            .IsRequired();

        builder.Property(track => track.BeatportSlug)
            .HasMaxLength(BeatportSlug.MaxLength)
            .IsRequired();

        builder.Property(track => track.Number)
            .IsRequired();

        builder.Property(track => track.Artist)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(track => track.Name)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(track => track.MixName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(track => track.Length)
            .IsRequired();

        builder.Property(track => track.SampleUri)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(track => track.CreatedDate)
            .IsRequired();

        builder.Property(track => track.ReleaseId)
            .IsRequired();

        builder.HasOne<Release>()
            .WithMany(release => release.Tracks)
            .HasForeignKey(track => track.ReleaseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(track => track.BeatportId)
            .IsUnique();
    }
}