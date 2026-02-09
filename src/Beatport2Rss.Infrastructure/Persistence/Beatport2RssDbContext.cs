using System.Reflection;

using Beatport2Rss.Domain.Feeds;
using Beatport2Rss.Domain.Releases;
using Beatport2Rss.Domain.Sessions;
using Beatport2Rss.Domain.Subscriptions;
using Beatport2Rss.Domain.Tags;
using Beatport2Rss.Domain.Tokens;
using Beatport2Rss.Domain.Tracks;
using Beatport2Rss.Domain.Users;
using Beatport2Rss.Infrastructure.Persistence.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Infrastructure.Persistence;

internal sealed class Beatport2RssDbContext(DbContextOptions<Beatport2RssDbContext> options) :
    DbContext(options)
{
    public DbSet<Feed> Feeds => this.Set<Feed>();
    public DbSet<FeedSubscription> FeedSubscriptions => this.Set<FeedSubscription>();
    public DbSet<Release> Releases => this.Set<Release>();
    public DbSet<Session> Sessions => this.Set<Session>();
    public DbSet<Subscription> Subscriptions => this.Set<Subscription>();
    public DbSet<SubscriptionTag> SubscriptionTags => this.Set<SubscriptionTag>();
    public DbSet<Tag> Tags => this.Set<Tag>();
    public DbSet<Token> Tokens => this.Set<Token>();
    public DbSet<Track> Tracks => this.Set<Track>();
    public DbSet<User> Users => this.Set<User>();

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