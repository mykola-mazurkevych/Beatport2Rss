using Beatport2Rss.Common.EntityFrameworkCore.Extensions;
using Beatport2Rss.ReleaseCollector.Domain.Artists;
using Beatport2Rss.ReleaseCollector.Domain.Labels;
using Beatport2Rss.ReleaseCollector.Domain.Releases;
using Beatport2Rss.ReleaseCollector.Domain.Tracks;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.ReleaseCollector.Infrastructure.Persistence;

internal sealed class ReleaseCollectorDbContext(DbContextOptions<ReleaseCollectorDbContext> options) :
    DbContext(options)
{
    private const string SchemaName = "collector";

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
        modelBuilder.HasDefaultSchema(SchemaName);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReleaseCollectorDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}