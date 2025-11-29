using Beatport2Rss.Domain.Releases;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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
            .IsRequired();

        builder.Property(release => release.BeatportSlug)
            .HasMaxLength(250)
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
            .HasConversion<EnumToStringConverter<ReleaseStatus>>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Navigation(release => release.Tracks)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(release => release.BeatportId)
            .IsUnique();
    }
}