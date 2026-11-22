using Beatport2Rss.Api.Domain.EmailConfirmations;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Api.Infrastructure.Persistence.ValueConverters.EmailConfirmations;

internal sealed class SecretHashValueConverter() :
    ValueConverter<SecretHash, string>(
        secretHash => secretHash.Value,
        value => SecretHash.Create(value));