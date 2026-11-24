using Beatport2Rss.Api.Infrastructure.Persistence.QueryModels;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Api.Infrastructure.Persistence.Configurations;

internal sealed class EmailConfirmationQueryModelConfiguration :
    IEntityTypeConfiguration<EmailConfirmationQueryModel>
{
    public void Configure(EntityTypeBuilder<EmailConfirmationQueryModel> builder)
    {
        builder.ToView("vwEmailConfirmations");

        builder.HasKey(emailConfirmationQueryModel => emailConfirmationQueryModel.Id);

        builder.Property(emailConfirmationQueryModel => emailConfirmationQueryModel.Id);
        builder.Property(emailConfirmationQueryModel => emailConfirmationQueryModel.ExpiresAt);
        builder.Property(emailConfirmationQueryModel => emailConfirmationQueryModel.ConfirmedAt);
    }
}