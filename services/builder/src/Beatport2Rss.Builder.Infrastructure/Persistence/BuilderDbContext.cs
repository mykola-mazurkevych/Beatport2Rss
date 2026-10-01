using Beatport2Rss.Builder.Domain.Feeds;
using Beatport2Rss.Builder.Domain.Releases;
using Beatport2Rss.Builder.Domain.Subscriptions;
using Beatport2Rss.Builder.Domain.Tracks;
using Beatport2Rss.Common.EntityFrameworkCore.Extensions;
using Beatport2Rss.Common.Messaging.Persistence.Entities;
using Beatport2Rss.Common.Messaging.Persistence.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Builder.Infrastructure.Persistence;

internal sealed class BuilderDbContext(DbContextOptions<BuilderDbContext> options) :
    DbContext(options), IInboxDbContext, IOutboxDbContext
{
    internal const string Schema = "builder";
    internal const string InboxSchema = "builder_inbox";
    internal const string OutboxSchema = "builder_outbox";

    public DbSet<Feed> Feeds => Set<Feed>();
    public DbSet<Release> Releases => Set<Release>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<Track> Tracks => Set<Track>();

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
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BuilderDbContext).Assembly);

        modelBuilder.Entity<InboxMessage>().Metadata.SetSchema(InboxSchema);
        modelBuilder.Entity<OutboxMessage>().Metadata.SetSchema(OutboxSchema);

        base.OnModelCreating(modelBuilder);
    }
}