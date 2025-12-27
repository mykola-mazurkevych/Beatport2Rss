using System.Reflection;

using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Feeds;
using Beatport2Rss.Domain.Releases;
using Beatport2Rss.Domain.Sessions;
using Beatport2Rss.Domain.Subscriptions;
using Beatport2Rss.Domain.Tags;
using Beatport2Rss.Domain.Tokens;
using Beatport2Rss.Domain.Tracks;
using Beatport2Rss.Domain.Users;
using Beatport2Rss.Infrastructure.Persistence.Entities;
using Beatport2Rss.Infrastructure.Persistence.ValueConverters;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Infrastructure.Persistence;

internal sealed class Beatport2RssDbContext(DbContextOptions<Beatport2RssDbContext> options)
    : DbContext(options)
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
        configurationBuilder.Properties<BeatportAccessToken>().HaveConversion<BeatportAccessTokenValueConverter>();
        configurationBuilder.Properties<BeatportId>().HaveConversion<BeatportIdValueConverter>();
        configurationBuilder.Properties<BeatportSlug>().HaveConversion<BeatportSlugValueConverter>();
        configurationBuilder.Properties<EmailAddress>().HaveConversion<EmailAddressValueConverter>();
        configurationBuilder.Properties<FeedId>().HaveConversion<FeedIdValueConverter>();
        configurationBuilder.Properties<FeedName>().HaveConversion<FeedNameValueConverter>();
        configurationBuilder.Properties<PasswordHash>().HaveConversion<PasswordHashValueConverter>();
        configurationBuilder.Properties<RefreshTokenHash>().HaveConversion<RefreshTokenHashValueConverter>();
        configurationBuilder.Properties<ReleaseId>().HaveConversion<ReleaseIdValueConverter>();
        configurationBuilder.Properties<SessionId>().HaveConversion<SessionIdValueConverter>();
        configurationBuilder.Properties<Slug>().HaveConversion<SlugValueConverter>();
        configurationBuilder.Properties<SubscriptionId>().HaveConversion<SubscriptionIdValueConverter>();
        configurationBuilder.Properties<TagId>().HaveConversion<TagIdValueConverter>();
        configurationBuilder.Properties<TagName>().HaveConversion<TagNameValueConverter>();
        configurationBuilder.Properties<TokenId>().HaveConversion<TokenIdValueConverter>();
        configurationBuilder.Properties<TrackId>().HaveConversion<TrackIdValueConverter>();
        configurationBuilder.Properties<Uri>().HaveConversion<UriValueConverter>();
        configurationBuilder.Properties<UserId>().HaveConversion<UserIdValueConverter>();

        base.ConfigureConventions(configurationBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}