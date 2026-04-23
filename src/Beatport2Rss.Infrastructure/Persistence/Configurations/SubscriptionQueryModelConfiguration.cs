using Beatport2Rss.Infrastructure.Persistence.Extensions;
using Beatport2Rss.Infrastructure.Persistence.QueryModels;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Infrastructure.Persistence.Configurations;

internal sealed class SubscriptionQueryModelConfiguration :
    IEntityTypeConfiguration<SubscriptionQueryModel>
{
    public void Configure(EntityTypeBuilder<SubscriptionQueryModel> builder)
    {
        builder.ToView("vwSubscriptions");

        builder.HasKey(subscriptionQueryModel => subscriptionQueryModel.Id);

        builder.Property(subscriptionQueryModel => subscriptionQueryModel.Id);
        builder.Property(subscriptionQueryModel => subscriptionQueryModel.CreatedAt);
        builder.Property(subscriptionQueryModel => subscriptionQueryModel.Name);
        builder.Property(subscriptionQueryModel => subscriptionQueryModel.Slug);
        builder.Property(subscriptionQueryModel => subscriptionQueryModel.BeatportType)
            .IsEnum();
        builder.Property(subscriptionQueryModel => subscriptionQueryModel.BeatportId);
        builder.Property(subscriptionQueryModel => subscriptionQueryModel.BeatportSlug);
        builder.Property(subscriptionQueryModel => subscriptionQueryModel.ImageUri);
        builder.Property(subscriptionQueryModel => subscriptionQueryModel.RefreshedAt);

        builder.HasMany(subscriptionQueryModel => subscriptionQueryModel.Tags)
            .WithOne()
            .HasForeignKey(subscriptionTagQueryModel => subscriptionTagQueryModel.SubscriptionId);
    }
}