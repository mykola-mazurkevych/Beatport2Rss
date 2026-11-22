using Beatport2Rss.Api.Domain.EmailVerifications;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Api.Infrastructure.Persistence.ValueConverters.EmailVerifications;

internal sealed class SecretHashValueConverter() :
    ValueConverter<SecretHash, string>(
        secretHash => secretHash.Value,
        value => SecretHash.Create(value));