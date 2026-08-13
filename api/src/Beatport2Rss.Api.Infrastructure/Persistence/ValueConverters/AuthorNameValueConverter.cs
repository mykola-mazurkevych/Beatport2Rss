using Beatport2Rss.Api.Domain.Feeds;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Api.Infrastructure.Persistence.ValueConverters;

internal sealed class AuthorNameValueConverter() :
    ValueConverter<AuthorName, string>(
        authorName => authorName.Value,
        value => AuthorName.Create(value));