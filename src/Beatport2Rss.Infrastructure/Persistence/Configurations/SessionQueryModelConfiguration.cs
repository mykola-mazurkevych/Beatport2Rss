using Beatport2Rss.Infrastructure.Persistence.QueryModels;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Infrastructure.Persistence.Configurations;

internal sealed class SessionQueryModelConfiguration : IEntityTypeConfiguration<SessionQueryModel>
{
    public void Configure(EntityTypeBuilder<SessionQueryModel> builder)
    {
        builder.ToView("vwSessions");

        builder.HasNoKey();

        builder.Property(sessionQueryModel => sessionQueryModel.Id);
        builder.Property(sessionQueryModel => sessionQueryModel.CreatedAt);
        builder.Property(sessionQueryModel => sessionQueryModel.UserId);
        builder.Property(sessionQueryModel => sessionQueryModel.EmailAddress);
        builder.Property(sessionQueryModel => sessionQueryModel.FirstName);
        builder.Property(sessionQueryModel => sessionQueryModel.LastName);
        builder.Property(sessionQueryModel => sessionQueryModel.UserAgent);
        builder.Property(sessionQueryModel => sessionQueryModel.IpAddress);
        builder.Property(sessionQueryModel => sessionQueryModel.RefreshTokenExpiresAt);
    }
}