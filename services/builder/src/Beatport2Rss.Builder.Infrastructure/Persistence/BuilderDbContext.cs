using Beatport2Rss.Builder.Domain.Feeds;
using Beatport2Rss.Builder.Domain.Releases;
using Beatport2Rss.Builder.Domain.Subscriptions;
using Beatport2Rss.Builder.Domain.Tags;
using Beatport2Rss.Builder.Domain.Tracks;
using Beatport2Rss.Common.EntityFrameworkCore.Extensions;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Builder.Infrastructure.Persistence;

internal sealed class BuilderDbContext(DbContextOptions<BuilderDbContext> options) :
    DbContext(options),
    IBeatport2RssDbContext
{
    public static string Schema => "builder";
    public const string SubscriptionTags = "SubscriptionTags";

    public DbSet<Feed> Feeds => Set<Feed>();
    public DbSet<FeedSubscription> FeedSubscriptions => Set<FeedSubscription>();
    public DbSet<Release> Releases => Set<Release>();
    public DbSet<ReleaseArtist> ReleaseArtists => Set<ReleaseArtist>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<Track> Tracks => Set<Track>();
    public DbSet<TrackArtist> TrackArtists => Set<TrackArtist>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.ConfigureConversions();

        base.ConfigureConventions(configurationBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BuilderDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}