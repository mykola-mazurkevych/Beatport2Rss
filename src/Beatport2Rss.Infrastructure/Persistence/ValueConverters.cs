// ReSharper disable UnusedMember.Global

using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Feeds;
using Beatport2Rss.Domain.Releases;
using Beatport2Rss.Domain.Sessions;
using Beatport2Rss.Domain.Subscriptions;
using Beatport2Rss.Domain.Tags;
using Beatport2Rss.Domain.Tokens;
using Beatport2Rss.Domain.Tracks;
using Beatport2Rss.Domain.Users;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Infrastructure.Persistence;

internal sealed class BeatportAccessTokenValueConverter() : ValueConverter<BeatportAccessToken, string>(beatportAccessToken => beatportAccessToken.Value, value => BeatportAccessToken.Create(value));
internal sealed class BeatportIdValueConverter() : ValueConverter<BeatportId, int>(beatportId => beatportId.Value, value => BeatportId.Create(value));
internal sealed class BeatportSlugValueConverter() : ValueConverter<BeatportSlug, string>(beatportSlug => beatportSlug.Value, value => BeatportSlug.Create(value));
internal sealed class EmailAddressValueConverter() : ValueConverter<EmailAddress, string>(emailAddress => emailAddress.Value, value => EmailAddress.Create(value));
internal sealed class FeedIdValueConverter() : ValueConverter<FeedId, Guid>(feedId => feedId.Value, value => FeedId.Create(value));
internal sealed class FeedNameValueConverter() : ValueConverter<FeedName, string>(feedName => feedName.Value, value => FeedName.Create(value));
internal sealed class PasswordHashValueConverter() : ValueConverter<PasswordHash, string>(passwordHash => passwordHash.Value, value => PasswordHash.Create(value));
internal sealed class RefreshTokenHashValueConverter() : ValueConverter<RefreshTokenHash, byte[]>(refreshToken => refreshToken.Value, value => RefreshTokenHash.Create(value));
internal sealed class ReleaseIdValueConverter() : ValueConverter<ReleaseId, int>(releaseId => releaseId.Value, value => ReleaseId.Create(value));
internal sealed class SessionIdValueConverter() : ValueConverter<SessionId, Guid>(sessionId => sessionId.Value, value => SessionId.Create(value));
internal sealed class SlugValueConverter() : ValueConverter<Slug, string>(slug => slug.Value, value => Slug.Create(value));
internal sealed class SubscriptionIdValueConverter() : ValueConverter<SubscriptionId, int>(subscriptionId => subscriptionId.Value, value => SubscriptionId.Create(value));
internal sealed class TagIdValueConverter() : ValueConverter<TagId, int>(tagId => tagId.Value, value => TagId.Create(value));
internal sealed class TagNameValueConverter() : ValueConverter<TagName, string>(tagName => tagName.Value, value => TagName.Create(value));
internal sealed class TokenIdValueConverter() : ValueConverter<TokenId, Guid>(tokenId => tokenId.Value, value => TokenId.Create(value));
internal sealed class TrackIdValueConverter() : ValueConverter<TrackId, int>(trackId => trackId.Value, value => TrackId.Create(value));
internal sealed class UriValueConverter() : ValueConverter<Uri, string>(uri => uri.ToString(), value => new Uri(value));
internal sealed class UserIdValueConverter() : ValueConverter<UserId, Guid>(userId => userId.Value, value => UserId.Create(value));