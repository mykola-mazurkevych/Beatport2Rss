using Beatport2Rss.Infrastructure.Persistence.QueryModels;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Infrastructure.Persistence.Configurations;

internal sealed class SubscriptionTagQueryModelConfiguration :
    IEntityTypeConfiguration<SubscriptionTagQueryModel>
{
    public void Configure(EntityTypeBuilder<SubscriptionTagQueryModel> builder)
    {
        builder.ToView("vwSubscriptionTags");

        builder.HasKey(subscriptionTagQueryModel => new
        {
            subscriptionTagQueryModel.SubscriptionId,
            subscriptionTagQueryModel.TagId,
        });

        builder.Property(subscriptionTagQueryModel => subscriptionTagQueryModel.SubscriptionId);
        builder.Property(subscriptionTagQueryModel => subscriptionTagQueryModel.TagId);
        builder.Property(subscriptionTagQueryModel => subscriptionTagQueryModel.UserId);
        builder.Property(subscriptionTagQueryModel => subscriptionTagQueryModel.Name);
        builder.Property(subscriptionTagQueryModel => subscriptionTagQueryModel.Slug);
    }
}