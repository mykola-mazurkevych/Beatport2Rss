using Beatport2Rss.Collector.Domain.Common.ValueObjects;
using Beatport2Rss.Collector.Domain.Labels;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Collector.Infrastructure.Persistence.Configurations;

internal sealed class LabelConfiguration :
    IEntityTypeConfiguration<Label>
{
    public void Configure(EntityTypeBuilder<Label> builder)
    {
        builder.ToTable(nameof(ReleaseCollectorDbContext.Labels));

        builder.HasKey(label => label.Id);

        builder.Property(label => label.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(label => label.CreatedAt)
            .IsRequired();

        builder.Property(label => label.Name)
            .HasMaxLength(LabelName.MaxLength)
            .IsRequired();

        builder.Property(label => label.BeatportId)
            .IsRequired();

        builder.Property(label => label.BeatportSlug)
            .HasMaxLength(BeatportSlug.MaxLength)
            .IsRequired();

        builder.HasIndex(label => label.BeatportId)
            .IsUnique();

        builder.HasIndex(label => label.BeatportSlug);
    }
}