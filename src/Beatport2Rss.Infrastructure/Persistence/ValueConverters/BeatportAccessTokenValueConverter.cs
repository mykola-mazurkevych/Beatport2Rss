using Beatport2Rss.Domain.Tokens;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Infrastructure.Persistence.ValueConverters;

internal sealed class BeatportAccessTokenValueConverter() :
    ValueConverter<BeatportAccessToken, string>(
        beatportAccessToken => beatportAccessToken.Value,
        value => BeatportAccessToken.Create(value));