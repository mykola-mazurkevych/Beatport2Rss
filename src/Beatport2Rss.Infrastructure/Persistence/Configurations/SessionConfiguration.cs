using Beatport2Rss.Domain.Sessions;
using Beatport2Rss.Domain.Users;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Infrastructure.Persistence.Configurations;

internal sealed class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.ToTable(nameof(Beatport2RssDbContext.Sessions));

        builder.HasKey(session => session.Id);

        builder.Property(session => session.UserId)
            .IsRequired();

        builder.Property(session => session.CreatedAt)
            .IsRequired();
        
        builder.Property(session => session.RefreshTokenHash)
            .HasColumnType("bytea")
            .IsRequired();
        builder.Property(session => session.RefreshTokenExpiresAt)
            .IsRequired();

        builder.Property(session => session.UserAgent)
            .HasMaxLength(Session.UserAgentMaxLength)
            .IsRequired(false);
        builder.Property(session => session.IpAddress)
            .HasMaxLength(Session.IpAddressMaxLength)
            .IsRequired(false);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(session => session.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}