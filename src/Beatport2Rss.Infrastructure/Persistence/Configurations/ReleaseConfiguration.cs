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

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasConversion(
                releaseId => releaseId.Value,
                value => ReleaseId.Create(value))
            .IsRequired();

        builder.Property(r => r.BeatportId)
            .IsRequired();

        builder.Property(r => r.BeatportSlug)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(r => r.Artist)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(r => r.Name)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(r => r.Label)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(r => r.CatalogNumber)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(r => r.ImageUri)
            .HasConversion(
                uri => uri.ToString(),
                value => new Uri(value))
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(r => r.ReleaseDate)
            .IsRequired();

        builder.Property(r => r.TracksCount)
            .IsRequired();

        builder.Property(r => r.CreatedDate)
            .IsRequired();

        builder.Property(r => r.Status)
            .HasConversion<EnumToStringConverter<ReleaseStatus>>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Navigation(r => r.Tracks)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(r => r.BeatportId)
            .IsUnique();
    }
}