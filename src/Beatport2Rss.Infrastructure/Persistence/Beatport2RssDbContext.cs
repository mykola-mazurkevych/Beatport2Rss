using Beatport2Rss.Common.EntityFrameworkCore.Extensions;
using Beatport2Rss.Domain.Countries;
using Beatport2Rss.Domain.Feeds;
using Beatport2Rss.Domain.Sessions;
using Beatport2Rss.Domain.Subscriptions;
using Beatport2Rss.Domain.Tags;
using Beatport2Rss.Domain.Users;
using Beatport2Rss.Infrastructure.Persistence.QueryModels;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Infrastructure.Persistence;

internal sealed class Beatport2RssDbContext(DbContextOptions<Beatport2RssDbContext> options) :
    DbContext(options)
{
    public DbSet<Country> Countries => Set<Country>();
    public DbSet<Feed> Feeds => Set<Feed>();
    public DbSet<FeedSubscription> FeedSubscriptions => Set<FeedSubscription>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<SubscriptionTag> SubscriptionTags => Set<SubscriptionTag>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<User> Users => Set<User>();

    public DbSet<FeedQueryModel> FeedQueryModels => Set<FeedQueryModel>();
    public DbSet<SessionQueryModel> SessionQueryModels => Set<SessionQueryModel>();
    public DbSet<SubscriptionQueryModel> SubscriptionQueryModels => Set<SubscriptionQueryModel>();
    public DbSet<SubscriptionTagQueryModel> SubscriptionTagQueryModels => Set<SubscriptionTagQueryModel>();
    public DbSet<TagQueryModel> TagQueryModels => Set<TagQueryModel>();
    public DbSet<UserQueryModel> UserQueryModels => Set<UserQueryModel>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.ConfigureConversions();

        base.ConfigureConventions(configurationBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(Beatport2RssDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}