using Beatport2Rss.Builder.Domain.Subscriptions;
using Beatport2Rss.Common.EntityFrameworkCore.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Builder.Infrastructure.Persistence.Configurations;

internal sealed class SubscriptionConfiguration :
    IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable(nameof(BuilderDbContext.Subscriptions));

        builder.HasKey(subscription => subscription.Id);

        builder.Property(subscription => subscription.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(subscription => subscription.CreatedAt)
            .IsRequired();

        builder.Property(subscription => subscription.Type)
            .IsEnum();

        builder.Property(subscription => subscription.Name)
            .HasMaxLength(SubscriptionName.MaxLength)
            .IsRequired();

        builder.Property(subscription => subscription.BeatportId)
            .IsRequired();

        builder.HasIndex(subscription => new { subscription.Type, subscription.BeatportId })
            .IsUnique();
    }
}