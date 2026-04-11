using Beatport2Rss.Infrastructure.Persistence.Extensions;
using Beatport2Rss.Infrastructure.QueryModels.Users;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Infrastructure.Persistence.Configurations;

internal sealed class UserQueryModelConfiguration : IEntityTypeConfiguration<UserQueryModel>
{
    public void Configure(EntityTypeBuilder<UserQueryModel> builder)
    {
        builder.ToView("vwUsers");

        builder.HasNoKey();

        builder.Property(userQueryModel => userQueryModel.Id);
        builder.Property(userQueryModel => userQueryModel.CreatedAt);
        builder.Property(userQueryModel => userQueryModel.EmailAddress);
        builder.Property(userQueryModel => userQueryModel.PasswordHash);
        builder.Property(userQueryModel => userQueryModel.FirstName);
        builder.Property(userQueryModel => userQueryModel.LastName);
        builder.Property(userQueryModel => userQueryModel.Status).IsEnum();
        builder.Property(userQueryModel => userQueryModel.IsActive);
        builder.Property(userQueryModel => userQueryModel.FeedsCount);
        builder.Property(userQueryModel => userQueryModel.TagsCount);
    }
}