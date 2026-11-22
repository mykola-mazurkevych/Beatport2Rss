using Beatport2Rss.Api.Domain.Users;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Api.Infrastructure.Persistence.ValueConverters;

internal sealed class EmailVerificationIdValueConverter() :
    ValueConverter<EmailVerificationId, Guid>(
        emailVerificationId => emailVerificationId.Value,
        value => EmailVerificationId.Create(value));
