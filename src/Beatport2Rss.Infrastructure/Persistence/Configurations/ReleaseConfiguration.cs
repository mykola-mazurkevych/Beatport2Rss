using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Releases;
using Beatport2Rss.Infrastructure.Persistence.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Infrastructure.Persistence.Configurations;

internal sealed class ReleaseConfiguration : IEntityTypeConfiguration<Release>
{
    public void Configure(EntityTypeBuilder<Release> builder)
    {
        builder.ToTable(nameof(Beatport2RssDbContext.Releases));

        builder.HasKey(release => release.Id);

        builder.Property(release => release.Id)
            .IsRequired();

        builder.Property(release => release.BeatportId)
            .IsRequired();

        builder.Property(release => release.BeatportSlug)
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