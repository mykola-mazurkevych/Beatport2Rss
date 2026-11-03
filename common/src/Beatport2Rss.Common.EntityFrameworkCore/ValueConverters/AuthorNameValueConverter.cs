using Beatport2Rss.Common.Domain.Feeds;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Common.EntityFrameworkCore.ValueConverters;

internal sealed class AuthorNameValueConverter() :
    ValueConverter<AuthorName, string>(
        authorName => authorName.Value,
        value => AuthorName.Create(value));