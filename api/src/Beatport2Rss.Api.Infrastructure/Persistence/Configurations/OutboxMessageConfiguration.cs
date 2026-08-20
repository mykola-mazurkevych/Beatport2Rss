using Beatport2Rss.Api.Infrastructure.Persistence.Outbox;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Api.Infrastructure.Persistence.Configurations;

internal sealed class OutboxMessageConfiguration :
    IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable(
            nameof(ApiDbContext.OutboxMessages),
            schema: ApiDbContext.OutboxSchema);

        builder.HasKey(message => message.Id);

        builder.Property(message => message.OccurredAt)
            .IsRequired();

        builder.Property(message => message.Type)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(message => message.Payload)
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(message => message.PublishedAt)
            .IsRequired(false);

        builder.Property(message => message.PublishAttempts)
            .IsRequired();

        builder.Property(message => message.LastError)
            .HasMaxLength(4_000)
            .IsRequired(false);

        builder.HasIndex(message => new { message.PublishedAt, message.OccurredAt });
    }
}