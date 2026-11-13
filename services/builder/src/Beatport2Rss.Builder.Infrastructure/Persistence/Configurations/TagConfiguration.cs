using Beatport2Rss.Builder.Domain.Tags;
using Beatport2Rss.Common.Domain.Tags;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Builder.Infrastructure.Persistence.Configurations;

internal sealed class TagConfiguration :
    IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable(nameof(BuilderDbContext.Tags));

        builder.HasKey(tag => tag.Id);

        builder.Property(tag => tag.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(tag => tag.CreatedAt)
            .IsRequired();

        builder.Property(tag => tag.Name)
            .HasMaxLength(TagName.MaxLength)
            .IsRequired();
    }
}
