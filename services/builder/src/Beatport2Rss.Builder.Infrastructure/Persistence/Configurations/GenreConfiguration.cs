using Beatport2Rss.Builder.Domain.Genres;
using Beatport2Rss.Common.Domain.Constants;

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

        builder.HasIndex(genre => genre.BeatportId)
            .IsUnique();

        builder.Property(genre => genre.Name)
            .HasMaxLength(MaxLengthConstants.Name)
            .IsRequired();
    }
}
