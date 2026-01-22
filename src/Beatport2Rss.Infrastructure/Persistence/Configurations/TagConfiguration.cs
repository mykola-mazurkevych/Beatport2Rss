using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Tags;
using Beatport2Rss.Domain.Users;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Infrastructure.Persistence.Configurations;

internal sealed class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable(nameof(Beatport2RssDbContext.Tags));

        builder.HasKey(tag => tag.Id);

        builder.Property(tag => tag.Id)
            .IsRequired();

        builder.Property(tag => tag.CreatedAt)
            .IsRequired();

        builder.Property(tag => tag.Name)
            .HasMaxLength(TagName.MaxLength)
            .IsRequired();

        builder.Property(tag => tag.Slug)
            .HasMaxLength(Slug.MaxLength)
            .IsRequired();

        builder.Property(tag => tag.UserId)
            .IsRequired();

        builder.HasOne<User>()
            .WithMany(user => user.Tags)
            .HasForeignKey(tag => tag.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(tag => tag.Slug)
            .IsUnique();

        builder.HasIndex(tag => new { tag.UserId, tag.Name })
            .IsUnique();
    }
}