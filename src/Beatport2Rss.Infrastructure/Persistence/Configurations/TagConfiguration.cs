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

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .HasConversion(
                tagId => tagId.Value,
                value => TagId.Create(value))
            .IsRequired();

        builder.Property(t => t.Name)
            .HasConversion(
                name => name.Value,
                value => TagName.Create(value))
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(t => t.Slug)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(t => t.CreatedDate)
            .IsRequired();

        builder.Property(t => t.UserId)
            .HasConversion(
                userId => userId.Value,
                value => UserId.Create(value))
            .IsRequired();

        builder.HasOne<User>()
            .WithMany(u => u.Tags)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(t => t.Slug)
            .IsUnique();

        builder.HasIndex(t => new { t.UserId, t.Name })
            .IsUnique();
    }
}