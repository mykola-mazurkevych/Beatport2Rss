using Beatport2Rss.Application.ReadModels.Users;
using Beatport2Rss.Infrastructure.Persistence.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Infrastructure.Persistence.Configurations;

internal sealed class UserReadModelConfiguration : IEntityTypeConfiguration<UserReadModel>
{
    public void Configure(EntityTypeBuilder<UserReadModel> builder)
    {
        builder.ToView("vwUsers");

        builder.HasNoKey();

        builder.Property(userReadModel => userReadModel.Id);
        builder.Property(userReadModel => userReadModel.CreatedAt);
        builder.Property(userReadModel => userReadModel.EmailAddress);
        builder.Property(userReadModel => userReadModel.PasswordHash);
        builder.Property(userReadModel => userReadModel.FirstName);
        builder.Property(userReadModel => userReadModel.LastName);
        builder.Property(userReadModel => userReadModel.Status).IsEnum();
        builder.Property(userReadModel => userReadModel.IsActive);
        builder.Property(userReadModel => userReadModel.FeedsCount);
        builder.Property(userReadModel => userReadModel.TagsCount);
    }
}