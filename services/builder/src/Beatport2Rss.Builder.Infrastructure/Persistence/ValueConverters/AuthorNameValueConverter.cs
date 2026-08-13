using Beatport2Rss.Builder.Domain.Feeds;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Builder.Infrastructure.Persistence.ValueConverters;

internal sealed class AuthorNameValueConverter() :
    ValueConverter<AuthorName, string>(
        authorName => authorName.Value,
        value => AuthorName.Create(value));