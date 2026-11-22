using Beatport2Rss.Api.Domain.EmailVerifications;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Api.Infrastructure.Persistence.ValueConverters.EmailVerifications;

internal sealed class EmailVerificationIdValueConverter() :
    ValueConverter<EmailVerificationId, Guid>(
        emailVerificationId => emailVerificationId.Value,
        value => EmailVerificationId.Create(value));