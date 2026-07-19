using Beatport2Rss.Builder.Domain.Artists;
using Beatport2Rss.Common.EntityFrameworkCore.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Builder.Infrastructure.Persistence.Configurations;

internal sealed class ArtistConfiguration :
    IEntityTypeConfiguration<Artist>
{
    public void Configure(EntityTypeBuilder<Artist> builder)
    {
        builder.ToTable(nameof(BuilderDbContext.Artists));

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

        builder.Property(artist => artist.BeatportUri)
            .IsUri();

        builder.HasIndex(artist => artist.BeatportId)
            .IsUnique();
    }
}