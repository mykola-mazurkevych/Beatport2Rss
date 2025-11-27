using Beatport2Rss.Domain.Feeds;
using Beatport2Rss.Domain.Users;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Infrastructure.Persistence.Configurations;

public sealed class FeedConfiguration : IEntityTypeConfiguration<Feed>
{
    public void Configure(EntityTypeBuilder<Feed> builder)
    {
        builder.ToTable(nameof(Beatport2RssDbContext.Feeds));

        builder.HasKey(f => f.Id);

        builder.Property(f => f.Id)
            .HasConversion(
                feedId => feedId.Value,
                value => FeedId.Create(value))
            .IsRequired();

        builder.Property(f => f.Name)
            .HasConversion(
                name => name.Value,
                value => FeedName.Create(value))
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(f => f.Slug)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(f => f.IsActive)
            .IsRequired();

        builder.Property(f => f.CreatedDate)
            .IsRequired();

        builder.Property(f => f.UserId)
            .HasConversion(
                userId => userId.Value,
                value => UserId.Create(value))
            .IsRequired();

        builder.HasOne<User>()
            .WithMany(u => u.Feeds)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(f => f.Slug)
            .IsUnique();

        builder.HasIndex(f => new { f.UserId, f.Name })
            .IsUnique();
    }
}