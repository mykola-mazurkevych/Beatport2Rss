using Beatport2Rss.Api.Domain.Users;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Api.Infrastructure.Persistence.ValueConverters;

internal sealed class EmailVerificationSecretHashValueConverter() :
    ValueConverter<EmailVerificationSecretHash, string>(
        secretHash => secretHash.Value,
        value => EmailVerificationSecretHash.Create(value));
