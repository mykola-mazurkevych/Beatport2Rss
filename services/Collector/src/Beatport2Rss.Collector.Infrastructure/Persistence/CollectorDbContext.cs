using Beatport2Rss.Collector.Domain.Artists;
using Beatport2Rss.Collector.Domain.Labels;
using Beatport2Rss.Collector.Domain.Releases;
using Beatport2Rss.Collector.Domain.Tracks;
using Beatport2Rss.Common.EntityFrameworkCore.Extensions;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Collector.Infrastructure.Persistence;

internal sealed class CollectorDbContext(DbContextOptions<CollectorDbContext> options) :
    DbContext(options)
{
    internal const string Schema = "collector";

    public DbSet<Artist> Artists => Set<Artist>();
    public DbSet<Label> Labels => Set<Label>();
    public DbSet<Release> Releases => Set<Release>();
    public DbSet<ReleaseArtist> ReleaseArtists => Set<ReleaseArtist>();
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
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CollectorDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}