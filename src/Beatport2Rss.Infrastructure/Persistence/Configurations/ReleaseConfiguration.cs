using Beatport2Rss.Domain.Common;
using Beatport2Rss.Domain.Releases;
using Beatport2Rss.Infrastructure.Persistence.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Infrastructure.Persistence.Configurations;

public sealed class ReleaseConfiguration : IEntityTypeConfiguration<Release>
{
    public void Configure(EntityTypeBuilder<Release> builder)
    {
        builder.ToTable(nameof(Beatport2RssDbContext.Releases));

        builder.HasKey(release => release.Id);

        builder.Property(release => release.Id)
            .HasConversion(
                releaseId => releaseId.Value,
                value => ReleaseId.Create(value))
            .IsRequired();

        builder.Property(release => release.BeatportId)
            .HasConversion(
                beatportId => beatportId.Value,
                value => BeatportId.Create(value))
            .IsRequired();

        builder.Property(release => release.BeatportSlug)
            .HasConversion(
                beatportSlug => beatportSlug.Value,
                value => BeatportSlug.Create(value))
            .HasMaxLength(BeatportSlug.MaxLength)
            .IsRequired();

        builder.Property(release => release.Artist)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(release => release.Name)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(release => release.Label)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(release => release.CatalogNumber)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(release => release.ImageUri)
            .HasConversion(
                uri => uri.ToString(),
                uriString => new Uri(uriString))
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(release => release.ReleaseDate)
            .IsRequired();

        builder.Property(release => release.TracksCount)
            .IsRequired();

        builder.Property(release => release.CreatedDate)
            .IsRequired();

        builder.Property(release => release.Status)
            .IsEnum();

        builder.Navigation(release => release.Tracks)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(release => release.BeatportId)
            .IsUnique();
    }
}