using Beatport2Rss.Domain.Sessions;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Infrastructure.Persistence.ValueConverters;

internal sealed class RefreshTokenHashValueConverter() : ValueConverter<RefreshTokenHash, byte[]>(refreshToken => refreshToken.Value, value => RefreshTokenHash.Create(value));