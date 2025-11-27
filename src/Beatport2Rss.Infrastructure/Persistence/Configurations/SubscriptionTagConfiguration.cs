using Beatport2Rss.Infrastructure.Persistence.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Infrastructure.Persistence.Configurations;

public sealed class SubscriptionTagConfiguration : IEntityTypeConfiguration<SubscriptionTag>
{
    public void Configure(EntityTypeBuilder<SubscriptionTag> builder)
    {
        builder.ToTable(nameof(Beatport2RssDbContext.SubscriptionTags));

        builder.HasKey(st => new { st.SubscriptionId, st.TagId });

        builder.Property(st => st.SubscriptionId)
            .IsRequired();

        builder.Property(st => st.TagId)
            .IsRequired();

        builder.Property(st => st.CreatedDate)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
    }
}