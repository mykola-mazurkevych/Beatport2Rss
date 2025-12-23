using Beatport2Rss.Domain.Tokens;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Infrastructure.Persistence.ValueConverters;

internal sealed class AccessTokenValueConverter() : ValueConverter<AccessToken, string>(accessToken => accessToken.Value, value => AccessToken.Create(value));