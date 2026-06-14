using Beatport2Rss.Common.EntityFrameworkCore.Extensions;
using Beatport2Rss.ReleaseCollector.Domain.Releases;
using Beatport2Rss.ReleaseCollector.Domain.Subscriptions;
using Beatport2Rss.ReleaseCollector.Domain.Tracks;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.ReleaseCollector.Infrastructure.Persistence;

internal sealed class ReleaseCollectorDbContext(DbContextOptions<ReleaseCollectorDbContext> options) :
    DbContext(options)
{
    private const string SchemaName = "collector";

    public DbSet<Release> Releases => Set<Release>();
    public DbSet<ReleaseSubscription> ReleaseSubscriptions => Set<ReleaseSubscription>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<Track> Tracks => Set<Track>();
    public DbSet<TrackSubscription> TrackSubscriptions => Set<TrackSubscription>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.ConfigureConversions();

        base.ConfigureConventions(configurationBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(SchemaName);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReleaseCollectorDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}