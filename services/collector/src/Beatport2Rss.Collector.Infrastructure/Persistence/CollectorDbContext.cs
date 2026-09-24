using Beatport2Rss.Collector.Domain.Subscriptions;
using Beatport2Rss.Common.EntityFrameworkCore.Extensions;
using Beatport2Rss.Common.Messaging.Persistence.Entities;
using Beatport2Rss.Common.Messaging.Persistence.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Collector.Infrastructure.Persistence;

internal sealed class CollectorDbContext(DbContextOptions<CollectorDbContext> options) :
    DbContext(options), IInboxDbContext, IOutboxDbContext
{
    internal const string Schema = "collector";
    internal const string InboxSchema = "collector_inbox";
    internal const string OutboxSchema = "collector_outbox";

    public DbSet<Subscription> Subscriptions => Set<Subscription>();

    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.ConfigureConversions();

        base.ConfigureConventions(configurationBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CollectorDbContext).Assembly);

        modelBuilder.Entity<InboxMessage>().Metadata.SetSchema(InboxSchema);
        modelBuilder.Entity<OutboxMessage>().Metadata.SetSchema(OutboxSchema);

        base.OnModelCreating(modelBuilder);
    }
}