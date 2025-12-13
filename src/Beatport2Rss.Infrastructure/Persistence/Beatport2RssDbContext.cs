using System.Reflection;

using Beatport2Rss.Domain.Feeds;
using Beatport2Rss.Domain.Releases;
using Beatport2Rss.Domain.Subscriptions;
using Beatport2Rss.Domain.Tags;
using Beatport2Rss.Domain.Tokens;
using Beatport2Rss.Domain.Tracks;
using Beatport2Rss.Domain.Users;
using Beatport2Rss.Infrastructure.Persistence.Entities;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Infrastructure.Persistence;

internal sealed class Beatport2RssDbContext(DbContextOptions<Beatport2RssDbContext> options)
    : DbContext(options)
{
    public DbSet<User> Users => this.Set<User>();
    public DbSet<Tag> Tags => this.Set<Tag>();

    public DbSet<Feed> Feeds => this.Set<Feed>();
    public DbSet<FeedSubscription> FeedSubscriptions => this.Set<FeedSubscription>();

    public DbSet<Subscription> Subscriptions => this.Set<Subscription>();
    public DbSet<SubscriptionTag> SubscriptionTags => this.Set<SubscriptionTag>();

    public DbSet<Release> Releases => this.Set<Release>();
    public DbSet<Track> Tracks => this.Set<Track>();

    public DbSet<Token> Tokens => this.Set<Token>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}