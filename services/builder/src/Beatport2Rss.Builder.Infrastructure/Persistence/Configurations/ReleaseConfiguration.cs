using Beatport2Rss.Builder.Domain.Artists;
using Beatport2Rss.Builder.Domain.Labels;
using Beatport2Rss.Builder.Domain.Releases;
using Beatport2Rss.Common.EntityFrameworkCore.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Builder.Infrastructure.Persistence.Configurations;

internal sealed class ReleaseConfiguration :
    IEntityTypeConfiguration<Release>
{
    public void Configure(EntityTypeBuilder<Release> builder)
    {
        builder.ToTable(nameof(BuilderDbContext.Releases));

        builder.HasKey(release => release.Id);

        builder.Property(release => release.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(release => release.CreatedAt)
            .IsRequired();

        builder.Property(release => release.BeatportId)
            .IsRequired();

        builder.Property(release => release.BeatportUri)
            .IsUri();

        builder.Property(release => release.Name)
            .HasMaxLength(ReleaseName.MaxLength)
            .IsRequired();

        builder.Property(release => release.LabelId)
            .IsRequired();

        builder.Property(release => release.ReleaseDate)
            .IsRequired();

        builder.Property(release => release.CatalogNumber)
            .HasMaxLength(CatalogNumber.MaxLength)
            .IsRequired();

        builder.Property(release => release.BeatportUri)
            .IsUri();

        builder.Property(release => release.ImageUri)
            .IsUri();

        builder.OwnsMany(
            release => release.Artists,
            navigationBuilder =>
            {
                navigationBuilder.ToTable(nameof(BuilderDbContext.ReleaseArtists));

                navigationBuilder.HasKey(releaseArtist => new { releaseArtist.ReleaseId, releaseArtist.ArtistId, releaseArtist.Type });

                navigationBuilder.Property(releaseArtist => releaseArtist.ReleaseId)
                    .IsRequired();

                navigationBuilder.Property(releaseArtist => releaseArtist.ArtistId)
                    .IsRequired();

                navigationBuilder.Property(releaseArtist => releaseArtist.Type)
                    .IsEnum();

                navigationBuilder.WithOwner()
                    .HasForeignKey(releaseArtist => releaseArtist.ReleaseId);

                navigationBuilder.HasOne<Artist>()
                    .WithMany()
                    .HasForeignKey(releaseArtist => releaseArtist.ArtistId);

                navigationBuilder.HasIndex(releaseArtist => releaseArtist.ArtistId);
            });

        builder.HasOne<Label>()
            .WithMany()
            .HasForeignKey(release => release.LabelId);

        builder.Navigation(release => release.Tracks)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(release => release.BeatportId)
            .IsUnique();
    }
}