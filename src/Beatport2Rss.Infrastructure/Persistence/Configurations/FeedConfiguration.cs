using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Feeds;
using Beatport2Rss.Domain.Users;
using Beatport2Rss.Infrastructure.Persistence.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Infrastructure.Persistence.Configurations;

internal sealed class FeedConfiguration : IEntityTypeConfiguration<Feed>
{
    private const string UserIdPropertyName = "UserId";

    public void Configure(EntityTypeBuilder<Feed> builder)
    {
        builder.ToTable(nameof(Beatport2RssDbContext.Feeds));

        builder.HasKey(feed => feed.Id);

        builder.Property(feed => feed.Id)
            .IsRequired();

        builder.Property(feed => feed.CreatedAt)
            .IsRequired();

        builder.Property(feed => feed.Name)
            .HasMaxLength(FeedName.MaxLength)
            .IsRequired();

        builder.Property(feed => feed.Slug)
            .HasMaxLength(Slug.MaxLength)
            .IsRequired();

        builder.Property(feed => feed.Status)
            .IsEnum();

        builder.Property(UserIdPropertyName)
            .IsRequired();

        builder.HasOne<User>()
            .WithMany(user => user.Feeds)
            .HasForeignKey(UserIdPropertyName)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(feed => feed.Slug)
            .IsUnique();

        builder.HasIndex(UserIdPropertyName, nameof(Feed.Name))
            .IsUnique();
    }
}