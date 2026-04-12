using Beatport2Rss.Domain.Tokens;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Infrastructure.Persistence.ValueConverters;

internal sealed class TokenIdValueConverter() :
    ValueConverter<TokenId, Guid>(
        tokenId => tokenId.Value,
        value => TokenId.Create(value));