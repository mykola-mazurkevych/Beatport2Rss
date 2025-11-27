using Beatport2Rss.Domain.Releases;
using Beatport2Rss.Domain.Tracks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Infrastructure.Persistence.Configurations;

public sealed class TrackConfiguration : IEntityTypeConfiguration<Track>
{
    public void Configure(EntityTypeBuilder<Track> builder)
    {
        builder.ToTable(nameof(Beatport2RssDbContext.Tracks));

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .HasConversion(
                trackId => trackId.Value,
                value => TrackId.Create(value))
            .IsRequired();

        builder.Property(t => t.BeatportId)
            .IsRequired();

        builder.Property(t => t.BeatportSlug)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(t => t.Number)
            .IsRequired();

        builder.Property(t => t.Artist)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(t => t.Name)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(t => t.MixName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(t => t.Length)
            .IsRequired();

        builder.Property(t => t.SampleUri)
            .HasConversion(
                uri => uri.ToString(),
                value => new Uri(value))
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(t => t.CreatedDate)
            .IsRequired();

        builder.Property(t => t.ReleaseId)
            .HasConversion(
                releaseId => releaseId.Value,
                value => ReleaseId.Create(value))
            .IsRequired();

        builder.HasOne<Release>()
            .WithMany(r => r.Tracks)
            .HasForeignKey(t => t.ReleaseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(t => t.BeatportId)
            .IsUnique();
    }
}