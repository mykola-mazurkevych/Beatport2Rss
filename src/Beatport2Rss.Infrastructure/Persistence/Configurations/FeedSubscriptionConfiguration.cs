using Beatport2Rss.Infrastructure.Persistence.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Infrastructure.Persistence.Configurations;

public sealed class FeedSubscriptionConfiguration : IEntityTypeConfiguration<FeedSubscription>
{
    public void Configure(EntityTypeBuilder<FeedSubscription> builder)
    {
        builder.ToTable(nameof(Beatport2RssDbContext.FeedSubscriptions));

        builder.HasKey(fs => new { fs.FeedId, fs.SubscriptionId });

        builder.Property(fs => fs.FeedId)
            .IsRequired();

        builder.Property(fs => fs.SubscriptionId)
            .IsRequired();

        builder.Property(fs => fs.CreatedDate)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
    }
}