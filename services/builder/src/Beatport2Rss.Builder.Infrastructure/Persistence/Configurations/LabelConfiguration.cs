using Beatport2Rss.Builder.Domain.Labels;
using Beatport2Rss.Common.EntityFrameworkCore.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Builder.Infrastructure.Persistence.Configurations;

internal sealed class LabelConfiguration :
    IEntityTypeConfiguration<Label>
{
    public void Configure(EntityTypeBuilder<Label> builder)
    {
        builder.ToTable(nameof(BuilderDbContext.Labels));

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
        
        builder.Property(label => label.BeatportUri)
            .IsUri();

        builder.HasIndex(label => label.BeatportId)
            .IsUnique();
    }
}