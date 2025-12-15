using Beatport2Rss.Domain.Common;
using Beatport2Rss.Domain.Feeds;
using Beatport2Rss.Domain.Users;
using Beatport2Rss.Infrastructure.Persistence.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Infrastructure.Persistence.Configurations;

public sealed class FeedConfiguration : IEntityTypeConfiguration<Feed>
{
    public void Configure(EntityTypeBuilder<Feed> builder)
    {
        builder.ToTable(nameof(Beatport2RssDbContext.Feeds));

        builder.HasKey(feed => feed.Id);

        builder.Property(feed => feed.Id)
            .HasConversion(
                feedId => feedId.Value,
                value => FeedId.Create(value))
            .IsRequired();
    
        builder.Property(feed => feed.Name)
            .HasConversion(
                feedName => feedName.Value,
                value => FeedName.Create(value))
            .HasMaxLength(FeedName.MaxLength)
            .IsRequired();

        builder.Property(feed => feed.Slug)
            .HasConversion(
                slug => slug.Value,
                value => Slug.Create(value))
            .HasMaxLength(Slug.MaxLength)
            .IsRequired();

        builder.Property(feed => feed.Status)
            .IsEnum();

        builder.Property(feed => feed.CreatedDate)
            .IsRequired();

        builder.Property(feed => feed.UserId)
            .HasConversion(
                userId => userId.Value,
                value => UserId.Create(value))
            .IsRequired();

        builder.HasOne<User>()
            .WithMany(user => user.Feeds)
            .HasForeignKey(feed => feed.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(feed => feed.Slug)
            .IsUnique();

        builder.HasIndex(feed => new { feed.UserId, feed.Name })
            .IsUnique();
    }
}