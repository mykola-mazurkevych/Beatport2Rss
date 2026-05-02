using System.Reflection;

using Beatport2Rss.Domain.Countries;
using Beatport2Rss.Domain.Feeds;
using Beatport2Rss.Domain.Releases;
using Beatport2Rss.Domain.Sessions;
using Beatport2Rss.Domain.Subscriptions;
using Beatport2Rss.Domain.Tags;
using Beatport2Rss.Domain.Tokens;
using Beatport2Rss.Domain.Tracks;
using Beatport2Rss.Domain.Users;
using Beatport2Rss.Infrastructure.Persistence.QueryModels;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Infrastructure.Persistence;

internal sealed class Beatport2RssDbContext(DbContextOptions<Beatport2RssDbContext> options) :
    DbContext(options)
{
    public DbSet<Country> Countries => Set<Country>();
    public DbSet<Feed> Feeds => Set<Feed>();
    public DbSet<FeedSubscription> FeedSubscriptions => Set<FeedSubscription>();
    public DbSet<Release> Releases => Set<Release>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<SubscriptionTag> SubscriptionTags => Set<SubscriptionTag>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<Token> Tokens => Set<Token>();
    public DbSet<Track> Tracks => Set<Track>();
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
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}

file static class ModelConfigurationBuilderExtensions
{
    extension(ModelConfigurationBuilder builder)
    {
        public void ConfigureConversions() =>
            typeof(Beatport2RssDbContext).Assembly
                .GetTypes()
                .Where(type =>
                    type.BaseType is not null &&
                    type.BaseType.IsGenericType &&
                    type.BaseType.GetGenericTypeDefinition() == typeof(ValueConverter<,>))
                .ToList()
                .ForEach(converterType => builder.Properties(converterType.BaseType!.GetGenericArguments()[0]).HaveConversion(converterType));
    }
}