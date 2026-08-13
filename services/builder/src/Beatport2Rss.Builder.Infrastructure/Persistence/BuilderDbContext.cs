using Beatport2Rss.Builder.Domain.Feeds;
using Beatport2Rss.Builder.Domain.Releases;
using Beatport2Rss.Builder.Domain.Subscriptions;
using Beatport2Rss.Builder.Domain.Tracks;
using Beatport2Rss.Common.EntityFrameworkCore.Extensions;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Builder.Infrastructure.Persistence;

internal sealed class BuilderDbContext(DbContextOptions<BuilderDbContext> options) :
    DbContext(options)
{
    internal const string Schema = "builder";

    public DbSet<Feed> Feeds => Set<Feed>();
    public DbSet<Release> Releases => Set<Release>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<Track> Tracks => Set<Track>();

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
