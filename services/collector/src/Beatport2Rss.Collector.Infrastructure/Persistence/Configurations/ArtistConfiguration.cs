using Beatport2Rss.Collector.Domain.Artists;
using Beatport2Rss.Collector.Domain.Common.ValueObjects;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Collector.Infrastructure.Persistence.Configurations;

internal sealed class ArtistConfiguration :
    IEntityTypeConfiguration<Artist>
{
    public void Configure(EntityTypeBuilder<Artist> builder)
    {
        builder.ToTable(nameof(CollectorDbContext.Artists));

        builder.HasKey(artist => artist.Id);

        builder.Property(artist => artist.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(artist => artist.CreatedAt)
            .IsRequired();

        builder.Property(artist => artist.Name)
            .HasMaxLength(ArtistName.MaxLength)
            .IsRequired();

        builder.Property(artist => artist.BeatportId)
            .IsRequired();

        builder.Property(artist => artist.BeatportSlug)
            .HasMaxLength(BeatportSlug.MaxLength)
            .IsRequired();

        builder.HasIndex(artist => artist.BeatportId)
            .IsUnique();

        builder.HasIndex(artist => artist.BeatportSlug);
    }
}