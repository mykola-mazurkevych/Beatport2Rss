using Beatport2Rss.Collector.Domain.Genres;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Collector.Infrastructure.Persistence.Configurations;

internal sealed class GenreConfiguration :
    IEntityTypeConfiguration<Genre>
{
    public void Configure(EntityTypeBuilder<Genre> builder)
    {
        builder.ToTable(nameof(CollectorDbContext.Genres));

        builder.HasKey(genre => genre.Id);

        builder.Property(genre => genre.Id)
            .IsRequired();

        builder.Property(genre => genre.CreatedAt)
            .IsRequired();

        builder.Property(genre => genre.BeatportId)
            .IsRequired();

        builder.HasIndex(genre => genre.BeatportId)
            .IsUnique();
    }
}