using Beatport2Rss.Domain.Users;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Infrastructure.Persistence.ValueConverters;

internal sealed class EmailAddressValueConverter() :
    ValueConverter<EmailAddress, string>(
        emailAddress => emailAddress.Value,
        value => EmailAddress.Create(value));