using Beatport2Rss.Collector.Domain.Feeds;
using Beatport2Rss.Collector.Domain.Releases;
using Beatport2Rss.Collector.Domain.Subscriptions;
using Beatport2Rss.Collector.Domain.Users;
using Beatport2Rss.Common.EntityFrameworkCore.Extensions;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Collector.Infrastructure.Persistence;

internal sealed class CollectorDbContext(DbContextOptions<CollectorDbContext> options) :
    DbContext(options), IBeatport2RssDbContext
{
    public static string Schema => "collector";

    public DbSet<Feed> Feeds => Set<Feed>();
    public DbSet<FeedSubscription> FeedSubscriptions => Set<FeedSubscription>();
    public DbSet<Release> Releases => Set<Release>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<User> Users => Set<User>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.ConfigureConversions();

        base.ConfigureConventions(configurationBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CollectorDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}