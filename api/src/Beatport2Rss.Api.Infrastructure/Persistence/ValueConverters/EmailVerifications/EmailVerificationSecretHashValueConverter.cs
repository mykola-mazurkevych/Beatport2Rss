using Beatport2Rss.Api.Domain.EmailVerifications;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Api.Infrastructure.Persistence.ValueConverters.EmailVerifications;

internal sealed class EmailVerificationSecretHashValueConverter() :
    ValueConverter<EmailVerificationSecretHash, string>(
        secretHash => secretHash.Value,
        value => EmailVerificationSecretHash.Create(value));
