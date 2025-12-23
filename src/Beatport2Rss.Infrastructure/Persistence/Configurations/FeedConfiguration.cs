using Beatport2Rss.Domain.Common.ValueObjects;
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
            .IsRequired();
    
        builder.Property(feed => feed.Name)
            .HasMaxLength(FeedName.MaxLength)
            .IsRequired();

        builder.Property(feed => feed.Slug)
            .HasMaxLength(Slug.MaxLength)
            .IsRequired();

        builder.Property(feed => feed.Status)
            .IsEnum();

        builder.Property(feed => feed.CreatedDate)
            .IsRequired();

        builder.Property(feed => feed.UserId)
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