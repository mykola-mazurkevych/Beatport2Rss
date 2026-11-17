using Beatport2Rss.Builder.Domain.Users;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Builder.Infrastructure.Persistence.Configurations;

internal sealed class UserConfiguration :
    IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(nameof(BuilderDbContext.Users));

        builder.HasKey(user => user.Id);

        builder.Property(user => user.Id)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(user => user.CreatedAt)
            .IsRequired();
    }
}