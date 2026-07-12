using Beatport2Rss.Api.Infrastructure.Persistence.QueryModels;
using Beatport2Rss.Common.EntityFrameworkCore.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Api.Infrastructure.Persistence.Configurations;

internal sealed class SubscriptionQueryModelConfiguration :
    IEntityTypeConfiguration<SubscriptionQueryModel>
{
    public void Configure(EntityTypeBuilder<SubscriptionQueryModel> builder)
    {
        builder.ToView("vwSubscriptions");

        builder.HasKey(subscriptionQueryModel => subscriptionQueryModel.Id);

        builder.Property(subscriptionQueryModel => subscriptionQueryModel.Id);
        builder.Property(subscriptionQueryModel => subscriptionQueryModel.CreatedAt);

        builder.Property(subscriptionQueryModel => subscriptionQueryModel.Type).IsEnum();
        builder.Property(subscriptionQueryModel => subscriptionQueryModel.Name);
        builder.Property(subscriptionQueryModel => subscriptionQueryModel.Slug);

        builder.Property(subscriptionQueryModel => subscriptionQueryModel.BeatportId);
        builder.Property(subscriptionQueryModel => subscriptionQueryModel.BeatportSlug);

        builder.Property(subscriptionQueryModel => subscriptionQueryModel.ImageUri);

        builder.Property(subscriptionQueryModel => subscriptionQueryModel.Country);

        builder.Property(subscriptionQueryModel => subscriptionQueryModel.SubscribersCount);

        builder.HasMany(subscriptionQueryModel => subscriptionQueryModel.Tags)
            .WithOne()
            .HasForeignKey(subscriptionTagQueryModel => subscriptionTagQueryModel.SubscriptionId);
    }
}