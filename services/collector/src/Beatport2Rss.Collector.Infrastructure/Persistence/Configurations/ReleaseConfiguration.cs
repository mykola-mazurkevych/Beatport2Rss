using Beatport2Rss.Collector.Domain.Releases;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Collector.Infrastructure.Persistence.Configurations;

internal sealed class ReleaseConfiguration :
    IEntityTypeConfiguration<Release>
{
    public void Configure(EntityTypeBuilder<Release> builder)
    {
        builder.ToTable(nameof(CollectorDbContext.Releases));

        builder.HasKey(release => release.Id);

        builder.Property(release => release.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(release => release.CreatedAt)
            .IsRequired();

        builder.Property(release => release.BeatportId)
            .IsRequired();

        builder.HasIndex(release => release.BeatportId)
            .IsUnique();
    }
}