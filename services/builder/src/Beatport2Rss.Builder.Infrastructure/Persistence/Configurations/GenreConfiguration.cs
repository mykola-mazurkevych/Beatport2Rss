using Beatport2Rss.Builder.Domain.Genres;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Builder.Infrastructure.Persistence.Configurations;

internal sealed class GenreConfiguration :
    IEntityTypeConfiguration<Genre>
{
    public void Configure(EntityTypeBuilder<Genre> builder)
    {
        builder.ToTable(nameof(BuilderDbContext.Genres));

        builder.HasKey(genre => genre.Id);

        builder.Property(genre => genre.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(genre => genre.CreatedAt)
            .IsRequired();

        builder.Property(genre => genre.BeatportId)
            .IsRequired();

        builder.Property(genre => genre.Name)
            .HasMaxLength(GenreName.MaxLength)
            .IsRequired();

        builder.HasIndex(genre => genre.BeatportId)
            .IsUnique();

        builder.HasIndex(genre => genre.Name)
            .IsUnique();
    }
}