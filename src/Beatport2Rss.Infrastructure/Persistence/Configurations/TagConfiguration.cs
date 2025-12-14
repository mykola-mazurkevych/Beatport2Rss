using Beatport2Rss.Domain.Common;
using Beatport2Rss.Domain.Tags;
using Beatport2Rss.Domain.Users;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Infrastructure.Persistence.Configurations;

public sealed class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable(nameof(Beatport2RssDbContext.Tags));

        builder.HasKey(tag => tag.Id);

        builder.Property(tag => tag.Id)
            .HasConversion(
                tagId => tagId.Value,
                value => TagId.Create(value))
            .IsRequired();

        builder.Property(tag => tag.Name)
            .HasConversion(
                name => name.Value,
                value => TagName.Create(value))
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(tag => tag.Slug)
            .HasConversion(
                slug => slug.Value,
                value => Slug.Create(value))
            .HasMaxLength(Slug.MaxLength)
            .IsRequired();

        builder.Property(tag => tag.CreatedDate)
            .IsRequired();

        builder.Property(tag => tag.UserId)
            .HasConversion(
                userId => userId.Value,
                value => UserId.Create(value))
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